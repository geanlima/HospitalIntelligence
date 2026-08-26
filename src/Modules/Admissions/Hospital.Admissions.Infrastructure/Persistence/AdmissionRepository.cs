using Hospital.Admissions.Application.Admissions.Abstractions;
using Hospital.Admissions.Domain.Admissions;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Admissions.Infrastructure.Persistence;

public sealed class AdmissionRepository
    : IAdmissionRepository
{
    private readonly AdmissionsDbContext _dbContext;

    public AdmissionRepository(
        AdmissionsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Admission?> GetByIdAsync(
        AdmissionId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Admissions
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Admission admission,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Admissions.AddAsync(
            admission,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateAsync(
        Admission admission,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Admission>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Admissions
            .AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.AdmissionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Admissions
            .AsNoTracking()
            .CountAsync(
                x => x.Status == AdmissionStatus.Active,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Admission>> SearchAsync(
    AdmissionStatus? status,
    string? unit,
    CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Admissions
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(
                x => x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(unit))
        {
            var normalizedUnit = unit.Trim();

            query = query.Where(
                x => x.Unit != null &&
                     EF.Functions.ILike(
                         x.Unit,
                         $"%{normalizedUnit}%"));
        }

        return await query
            .OrderByDescending(x => x.AdmissionDate)
            .ToListAsync(cancellationToken);
    }

}