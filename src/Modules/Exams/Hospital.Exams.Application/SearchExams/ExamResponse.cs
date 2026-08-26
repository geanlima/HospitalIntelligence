namespace Hospital.Exams.Application.Exams.SearchExams;

public sealed record ExamResponse(
    Guid Id,
    Guid PatientId,
    string Name,
    DateTimeOffset RequestedAtUtc,
    DateTimeOffset? ResultedAtUtc,
    string Status,
    string? Result);
