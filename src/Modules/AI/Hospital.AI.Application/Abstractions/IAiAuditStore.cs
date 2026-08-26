namespace Hospital.AI.Application.Abstractions;

public sealed record AiSourceCitation(
    string SourceId,
    string Title,
    string Excerpt,
    double Score);

public sealed record AiInteractionRecord(
    Guid Id,
    string PromptKey,
    string Question,
    string Answer,
    string Provider,
    Guid? PatientId,
    IReadOnlyList<AiSourceCitation> Sources,
    DateTimeOffset OccurredAtUtc);

public interface IAiAuditStore
{
    Task SaveAsync(
        AiInteractionRecord record,
        CancellationToken cancellationToken = default);
}
