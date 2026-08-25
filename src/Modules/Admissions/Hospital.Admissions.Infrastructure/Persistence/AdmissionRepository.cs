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
}