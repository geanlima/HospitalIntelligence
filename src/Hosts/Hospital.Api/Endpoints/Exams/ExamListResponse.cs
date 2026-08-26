namespace Hospital.Api.Endpoints.Exams;

public sealed record ExamListResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    string Name,
    DateTimeOffset RequestedAtUtc,
    DateTimeOffset? ResultedAtUtc,
    string Status,
    string? Result);
