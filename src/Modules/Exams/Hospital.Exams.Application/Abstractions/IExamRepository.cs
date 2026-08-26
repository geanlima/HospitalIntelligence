using Hospital.Exams.Domain.Exams;

namespace Hospital.Exams.Application.Exams.Abstractions;

public interface IExamRepository
{
    Task<Exam?> GetByIdAsync(
        ExamId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Exam exam,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Exam exam,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Exam>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<int> CountPendingAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Exam>> SearchAsync(
        ExamStatus? status,
        string? name,
        CancellationToken cancellationToken = default);
}