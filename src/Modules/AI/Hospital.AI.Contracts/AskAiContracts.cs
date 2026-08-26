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

public sealed record ChartAuditFindingDto(
    string Code,
    string Category,
    string Severity,
    string Title,
    string Message,
    IReadOnlyList<string> RelatedSourceIds);

public sealed record AuditPatientChartResponse(
    Guid PatientId,
    DateTimeOffset AuditedAtUtc,
    string OverallRisk,
    string Summary,
    int MissingDocumentationCount,
    int DivergenceCount,
    int FinancialGlosaRiskCount,
    IReadOnlyList<ChartAuditFindingDto> Findings);

public sealed record ClinicalSafetyFindingDto(
    string Code,
    string Category,
    string Severity,
    string Title,
    string Message,
    IReadOnlyList<string> RelatedSourceIds);

public sealed record AssessClinicalSafetyResponse(
    Guid PatientId,
    DateTimeOffset AssessedAtUtc,
    string OverallRisk,
    string Summary,
    bool DischargeReady,
    int DischargeBlockerCount,
    int DeteriorationScore,
    string DeteriorationBand,
    string TriageRecommendation,
    int MedicationIssueCount,
    IReadOnlyList<ClinicalSafetyFindingDto> Findings);

public sealed record StructureVoiceNoteRequest(
    string Transcript,
    Guid? PatientId = null,
    string NoteType = "Evolution");

public sealed record StructureVoiceNoteResponse(
    string DraftTitle,
    string StructuredContent,
    string NoteType,
    string Provider,
    Guid? PatientId);
