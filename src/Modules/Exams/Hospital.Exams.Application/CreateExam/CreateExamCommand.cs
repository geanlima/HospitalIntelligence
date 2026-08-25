namespace Hospital.Exams.Application.Exams.CreateExam;

public sealed record CreateExamCommand(
    Guid PatientId,
    string Name,
    DateTimeOffset RequestedAtUtc);