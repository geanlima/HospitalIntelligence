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

    public async Task<IReadOnlyCollection<Exam>> SearchAsync(
        ExamStatus? status,
        string? name,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Exams
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(
                x => x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName = name.Trim();

            query = query.Where(
                x => EF.Functions.ILike(
                    x.Name,
                    $"%{normalizedName}%"));
        }

        return await query
            .OrderByDescending(x => x.RequestedAtUtc)
            .ToListAsync(cancellationToken);
    }
}