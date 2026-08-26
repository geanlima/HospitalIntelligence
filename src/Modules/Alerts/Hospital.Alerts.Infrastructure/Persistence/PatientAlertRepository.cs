using Hospital.Alerts.Application.Alerts.Abstractions;
using Hospital.Alerts.Domain.Alerts;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Alerts.Infrastructure.Persistence;

public sealed class PatientAlertRepository
    : IPatientAlertRepository
{
    private readonly AlertsDbContext _dbContext;

    public PatientAlertRepository(
        AlertsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PatientAlert?> GetByIdAsync(
        PatientAlertId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Alerts
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        PatientAlert alert,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Alerts.AddAsync(
            alert,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateAsync(
        PatientAlert alert,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<PatientAlert>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Alerts
            .AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountCriticalAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Alerts
            .AsNoTracking()
            .CountAsync(
                x => x.Severity == AlertSeverity.Critical,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<PatientAlert>> SearchAsync(
        AlertStatus? status,
        AlertSeverity? severity,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Alerts
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(
                x => x.Status == status.Value);
        }

        if (severity.HasValue)
        {
            query = query.Where(
                x => x.Severity == severity.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}