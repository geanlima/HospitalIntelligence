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
