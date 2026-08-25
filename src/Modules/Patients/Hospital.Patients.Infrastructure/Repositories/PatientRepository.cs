using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;
using Hospital.Patients.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Patients.Infrastructure.Repositories;

public sealed class PatientRepository
    : IPatientRepository
{
    private readonly PatientsDbContext _context;

    public PatientRepository(
        PatientsDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetByIdAsync(
        PatientId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Patient?> GetByExternalIdAsync(
        string sourceSystem,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var normalizedSourceSystem =
            sourceSystem.Trim();

        var normalizedExternalId =
            externalId.Trim();

        return await _context.Patients
            .FirstOrDefaultAsync(
                x =>
                    x.ExternalIdentifier != null &&
                    x.ExternalIdentifier.SourceSystem == normalizedSourceSystem &&
                    x.ExternalIdentifier.ExternalId == normalizedExternalId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Patient>> SearchAsync(
        string? name,
        string? sourceSystem,
        CancellationToken cancellationToken = default)
    {
        var query =
            _context.Patients
                .AsNoTracking()
                .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName =
                name.Trim();

            query = query.Where(
                x => EF.Functions.ILike(
                    x.Name,
                    $"%{normalizedName}%"));
        }

        if (!string.IsNullOrWhiteSpace(sourceSystem))
        {
            var normalizedSourceSystem =
                sourceSystem.Trim();

            query = query.Where(
                x =>
                    x.ExternalIdentifier != null &&
                    EF.Functions.ILike(
                        x.ExternalIdentifier.SourceSystem,
                        normalizedSourceSystem));
        }

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(
                cancellationToken);
    }

    public async Task AddAsync(
        Patient patient,
        CancellationToken cancellationToken = default)
    {
        await _context.Patients.AddAsync(
            patient,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateAsync(
        Patient patient,
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(
            cancellationToken);
    }
}