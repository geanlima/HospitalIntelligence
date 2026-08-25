using Hospital.Prescriptions.Application.Abstractions;
using Hospital.Prescriptions.Domain.Prescriptions;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Prescriptions.Infrastructure.Persistence;

public sealed class PrescriptionRepository
    : IPrescriptionRepository
{
    private readonly PrescriptionsDbContext _dbContext;

    public PrescriptionRepository(
        PrescriptionsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Prescription?> GetByIdAsync(
        PrescriptionId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Prescriptions
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Prescription prescription,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Prescriptions.AddAsync(
            prescription,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateAsync(
        Prescription prescription,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Prescription>>
        GetByPatientIdAsync(
            Guid patientId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.Prescriptions
            .AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.PrescribedAtUtc)
            .ToListAsync(cancellationToken);
    }
}