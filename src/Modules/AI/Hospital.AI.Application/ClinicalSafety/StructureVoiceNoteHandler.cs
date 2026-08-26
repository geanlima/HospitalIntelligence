using Hospital.AI.Application.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.AI.Application.ClinicalSafety;

public sealed record StructureVoiceNoteCommand(
    string Transcript,
    Guid? PatientId,
    string NoteType);

public sealed record StructureVoiceNoteResult(
    string DraftTitle,
    string StructuredContent,
    string NoteType,
    string Provider,
    Guid? PatientId);

/// <summary>
/// Fase 17 — "Prontuário por Voz" sem hardware:
/// recebe transcrição (colar texto / STT externo) e estrutura um rascunho de nota.
/// </summary>
public sealed class StructureVoiceNoteHandler
{
    private readonly IAiAccessPolicy _accessPolicy;
    private readonly IAiGuardrail _guardrail;
    private readonly IPromptCatalog _promptCatalog;
    private readonly ILlmProvider _llmProvider;

    public StructureVoiceNoteHandler(
        IAiAccessPolicy accessPolicy,
        IAiGuardrail guardrail,
        IPromptCatalog promptCatalog,
        ILlmProvider llmProvider)
    {
        _accessPolicy = accessPolicy;
        _guardrail = guardrail;
        _promptCatalog = promptCatalog;
        _llmProvider = llmProvider;
    }

    public async Task<Result<StructureVoiceNoteResult>> HandleAsync(
        StructureVoiceNoteCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Transcript))
        {
            return Result<StructureVoiceNoteResult>.Failure(
                new Error(
                    "AI.VoiceNote.TranscriptEmpty",
                    "A transcrição não pode ser vazia."));
        }

        if (command.PatientId is Guid patientId && patientId != Guid.Empty)
        {
            var access =
                await _accessPolicy.EnsureCanAccessPatientAsync(
                    patientId,
                    cancellationToken);

            if (access.IsFailure)
            {
                return Result<StructureVoiceNoteResult>.Failure(access.Error);
            }
        }

        var inputGuard = _guardrail.ValidateInput(command.Transcript);
        if (!inputGuard.IsAllowed)
        {
            return Result<StructureVoiceNoteResult>.Failure(
                new Error(
                    "AI.Guardrail.InputBlocked",
                    inputGuard.Reason ?? "Transcrição bloqueada pelos guardrails."));
        }

        PromptTemplate prompt;
        try
        {
            prompt = _promptCatalog.GetRequired("voice-note-draft");
        }
        catch (KeyNotFoundException)
        {
            return Result<StructureVoiceNoteResult>.Failure(
                new Error(
                    "AI.Prompt.NotFound",
                    "Prompt 'voice-note-draft' não encontrado."));
        }

        var noteType = string.IsNullOrWhiteSpace(command.NoteType)
            ? "Evolution"
            : command.NoteType.Trim();

        var userPrompt = prompt.UserTemplate
            .Replace("{{transcript}}", command.Transcript.Trim(), StringComparison.Ordinal)
            .Replace("{{noteType}}", noteType, StringComparison.Ordinal);

        var completion =
            await _llmProvider.CompleteAsync(
                new LlmCompletionRequest(
                [
                    new LlmMessage("system", prompt.SystemInstruction),
                    new LlmMessage("user", userPrompt)
                ]),
                cancellationToken);

        var outputGuard = _guardrail.ValidateOutput(completion.Content);
        if (!outputGuard.IsAllowed)
        {
            return Result<StructureVoiceNoteResult>.Failure(
                new Error(
                    "AI.Guardrail.OutputBlocked",
                    outputGuard.Reason ?? "Rascunho bloqueado pelos guardrails."));
        }

        return Result<StructureVoiceNoteResult>.Success(
            new StructureVoiceNoteResult(
                $"Rascunho de nota ({noteType})",
                completion.Content.Trim(),
                noteType,
                completion.ProviderName,
                command.PatientId));
    }
}
