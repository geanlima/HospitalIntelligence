using Hospital.Exams.Domain.Exams;

namespace Hospital.Exams.Application.Exams.SearchExams;

public sealed record SearchExamsQuery(
    ExamStatus? Status,
    string? Name);
