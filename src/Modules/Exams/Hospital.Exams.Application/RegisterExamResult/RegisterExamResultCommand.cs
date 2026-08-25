using Hospital.Exams.Domain.Exams;

namespace Hospital.Exams.Application.Exams.RegisterExamResult;

public sealed record RegisterExamResultCommand(
    ExamId ExamId,
    string Result,
    DateTimeOffset ResultedAtUtc);