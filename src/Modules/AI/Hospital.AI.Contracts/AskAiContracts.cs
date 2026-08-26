namespace Hospital.AI.Contracts;

public sealed record AskAiRequest(
    string Question,
    Guid? PatientId = null,
    string PromptKey = "clinical-assistant");

public sealed record AiSourceDto(
    string SourceId,
    string Title,
    string Excerpt,
    double Score);

public sealed record AskAiResponse(
    string Answer,
    string PromptKey,
    string Provider,
    IReadOnlyList<AiSourceDto> Sources,
    Guid InteractionId,
    DateTimeOffset OccurredAtUtc);

public sealed record IndexPatientClinicalRecordsResponse(
    Guid PatientId,
    int IndexedCount,
    DateTimeOffset IndexedAtUtc);

public sealed record SearchClinicalKnowledgeRequest(
    string Query,
    Guid PatientId,
    int TopK = 5);

public sealed record ClinicalKnowledgeHitDto(
    string SourceId,
    string Title,
    string Excerpt,
    double Score);

public sealed record SearchClinicalKnowledgeResponse(
    Guid PatientId,
    string Query,
    IReadOnlyList<ClinicalKnowledgeHitDto> Hits);
