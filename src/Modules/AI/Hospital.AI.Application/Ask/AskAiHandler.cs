using Hospital.AI.Application.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.AI.Application.Ask;

public sealed record AskAiQuery(
    string Question,
    Guid? PatientId,
    string PromptKey);

public sealed record AskAiResult(
    string Answer,
    string PromptKey,
    string Provider,
    IReadOnlyList<AiSourceCitation> Sources,
    Guid InteractionId,
    DateTimeOffset OccurredAtUtc);

public sealed class AskAiHandler
{
    private static readonly HashSet<string> PatientScopedPrompts =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "clinical-chart-qa",
            "patient-summary"
        };

    private readonly IAiAccessPolicy _accessPolicy;
    private readonly IAiGuardrail _guardrail;
    private readonly IPromptCatalog _promptCatalog;
    private readonly IRagRetriever _ragRetriever;
    private readonly ILlmProvider _llmProvider;
    private readonly IAiAuditStore _auditStore;

    public AskAiHandler(
        IAiAccessPolicy accessPolicy,
        IAiGuardrail guardrail,
        IPromptCatalog promptCatalog,
        IRagRetriever ragRetriever,
        ILlmProvider llmProvider,
        IAiAuditStore auditStore)
    {
        _accessPolicy = accessPolicy;
        _guardrail = guardrail;
        _promptCatalog = promptCatalog;
        _ragRetriever = ragRetriever;
        _llmProvider = llmProvider;
        _auditStore = auditStore;
    }

    public async Task<Result<AskAiResult>> HandleAsync(
        AskAiQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Question))
        {
            return Result<AskAiResult>.Failure(
                new Error(
                    "AI.Question.Empty",
                    "A pergunta não pode ser vazia."));
        }

        var requiresPatient =
            PatientScopedPrompts.Contains(query.PromptKey) ||
            query.PatientId.HasValue;

        if (requiresPatient)
        {
            if (query.PatientId is null || query.PatientId == Guid.Empty)
            {
                return Result<AskAiResult>.Failure(
                    new Error(
                        "AI.Ask.PatientIdRequired",
                        "PatientId é obrigatório para perguntas sobre o prontuário."));
            }

            var access =
                await _accessPolicy.EnsureCanAccessPatientAsync(
                    query.PatientId.Value,
                    cancellationToken);

            if (access.IsFailure)
            {
                return Result<AskAiResult>.Failure(access.Error);
            }
        }

        var inputGuard =
            _guardrail.ValidateInput(query.Question);

        if (!inputGuard.IsAllowed)
        {
            return Result<AskAiResult>.Failure(
                new Error(
                    "AI.Guardrail.InputBlocked",
                    inputGuard.Reason ?? "Pergunta bloqueada pelos guardrails."));
        }

        PromptTemplate prompt;

        try
        {
            prompt = _promptCatalog.GetRequired(query.PromptKey);
        }
        catch (KeyNotFoundException)
        {
            return Result<AskAiResult>.Failure(
                new Error(
                    "AI.Prompt.NotFound",
                    $"Prompt '{query.PromptKey}' não encontrado."));
        }

        var ragContext =
            await _ragRetriever.RetrieveAsync(
                query.Question,
                query.PatientId,
                topK: 5,
                cancellationToken);

        var contextBlock = string.Join(
            Environment.NewLine + Environment.NewLine,
            ragContext.Chunks.Select(
                (c, index) =>
                    $"[{index + 1}] {c.Chunk.Title} ({c.Chunk.SourceId})\n{c.Chunk.Content}"));

        if (string.IsNullOrWhiteSpace(contextBlock))
        {
            contextBlock =
                "(Nenhum trecho indexado encontrado para este paciente. " +
                "Oriente indexar o prontuário antes de responder.)";
        }

        var userPrompt = prompt.UserTemplate
            .Replace("{{question}}", query.Question.Trim(), StringComparison.Ordinal)
            .Replace("{{context}}", contextBlock, StringComparison.Ordinal);

        var completion =
            await _llmProvider.CompleteAsync(
                new LlmCompletionRequest(
                [
                    new LlmMessage("system", prompt.SystemInstruction),
                    new LlmMessage("user", userPrompt)
                ]),
                cancellationToken);

        var outputGuard =
            _guardrail.ValidateOutput(completion.Content);

        if (!outputGuard.IsAllowed)
        {
            return Result<AskAiResult>.Failure(
                new Error(
                    "AI.Guardrail.OutputBlocked",
                    outputGuard.Reason ?? "Resposta bloqueada pelos guardrails."));
        }

        var sources = ragContext.Chunks
            .Select(c => new AiSourceCitation(
                c.Chunk.SourceId,
                c.Chunk.Title,
                Truncate(c.Chunk.Content, 180),
                c.Score))
            .ToList()
            .AsReadOnly();

        var interactionId = Guid.NewGuid();
        var occurredAt = DateTimeOffset.UtcNow;

        await _auditStore.SaveAsync(
            new AiInteractionRecord(
                interactionId,
                query.PromptKey,
                query.Question.Trim(),
                completion.Content,
                completion.ProviderName,
                query.PatientId,
                sources,
                occurredAt),
            cancellationToken);

        return Result<AskAiResult>.Success(
            new AskAiResult(
                completion.Content,
                query.PromptKey,
                completion.ProviderName,
                sources,
                interactionId,
                occurredAt));
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength] + "...";
    }
}
