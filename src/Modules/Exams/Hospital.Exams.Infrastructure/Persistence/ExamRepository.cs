using Hospital.Exams.Application.Exams.Abstractions;
using Hospital.Exams.Domain.Exams;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Exams.Infrastructure.Persistence;

public sealed class ExamRepository
    : IExamRepository
{
    private readonly ExamsDbContext _dbContext;

    public ExamRepository(
        ExamsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Exam?> GetByIdAsync(
        ExamId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Exams
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Exam exam,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Exams.AddAsync(
            exam,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateAsync(
        Exam exam,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Exam>>
        GetByPatientIdAsync(
            Guid patientId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.Exams
            .AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.RequestedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountPendingAsync(
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Exams
            .AsNoTracking()
            .CountAsync(
                x =>
                    x.Status == ExamStatus.Requested ||
                    x.Status == ExamStatus.InProgress,
                cancellationToken);
    }
}