using Hospital.AI.Application.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.AI.Application.Search;

public sealed record SearchClinicalKnowledgeQuery(
    string Query,
    Guid PatientId,
    int TopK = 5);

public sealed record ClinicalKnowledgeHit(
    string SourceId,
    string Title,
    string Excerpt,
    double Score);

public sealed record SearchClinicalKnowledgeResult(
    Guid PatientId,
    string Query,
    IReadOnlyList<ClinicalKnowledgeHit> Hits);

public sealed class SearchClinicalKnowledgeHandler
{
    private readonly IAiAccessPolicy _accessPolicy;
    private readonly IAiGuardrail _guardrail;
    private readonly IRagRetriever _ragRetriever;

    public SearchClinicalKnowledgeHandler(
        IAiAccessPolicy accessPolicy,
        IAiGuardrail guardrail,
        IRagRetriever ragRetriever)
    {
        _accessPolicy = accessPolicy;
        _guardrail = guardrail;
        _ragRetriever = ragRetriever;
    }

    public async Task<Result<SearchClinicalKnowledgeResult>> HandleAsync(
        SearchClinicalKnowledgeQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.PatientId == Guid.Empty)
        {
            return Result<SearchClinicalKnowledgeResult>.Failure(
                new Error(
                    "AI.Search.PatientIdRequired",
                    "PatientId é obrigatório para busca no prontuário."));
        }

        if (string.IsNullOrWhiteSpace(query.Query))
        {
            return Result<SearchClinicalKnowledgeResult>.Failure(
                new Error(
                    "AI.Search.QueryEmpty",
                    "A consulta não pode ser vazia."));
        }

        var access =
            await _accessPolicy.EnsureCanAccessPatientAsync(
                query.PatientId,
                cancellationToken);

        if (access.IsFailure)
        {
            return Result<SearchClinicalKnowledgeResult>.Failure(
                access.Error);
        }

        var inputGuard = _guardrail.ValidateInput(query.Query);

        if (!inputGuard.IsAllowed)
        {
            return Result<SearchClinicalKnowledgeResult>.Failure(
                new Error(
                    "AI.Guardrail.InputBlocked",
                    inputGuard.Reason ?? "Consulta bloqueada pelos guardrails."));
        }

        var topK = Math.Clamp(query.TopK, 1, 20);

        var rag =
            await _ragRetriever.RetrieveAsync(
                query.Query.Trim(),
                query.PatientId,
                topK,
                cancellationToken);

        var hits = rag.Chunks
            .Select(c => new ClinicalKnowledgeHit(
                c.Chunk.SourceId,
                c.Chunk.Title,
                Truncate(c.Chunk.Content, 240),
                c.Score))
            .ToList()
            .AsReadOnly();

        return Result<SearchClinicalKnowledgeResult>.Success(
            new SearchClinicalKnowledgeResult(
                query.PatientId,
                query.Query.Trim(),
                hits));
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
