using Hospital.VitalSigns.Application.VitalSigns.Abstractions;
using Hospital.VitalSigns.Domain.VitalSigns;
using Microsoft.EntityFrameworkCore;

namespace Hospital.VitalSigns.Infrastructure.Persistence;

public sealed class VitalSignRepository
    : IVitalSignRepository
{
    private readonly VitalSignsDbContext _dbContext;

    public VitalSignRepository(
        VitalSignsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<VitalSign?> GetByIdAsync(
        VitalSignId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.VitalSigns
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        VitalSign vitalSign,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.VitalSigns.AddAsync(
            vitalSign,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<VitalSign>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.VitalSigns
            .AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.MeasuredAtUtc)
            .ToListAsync(cancellationToken);
    }
}