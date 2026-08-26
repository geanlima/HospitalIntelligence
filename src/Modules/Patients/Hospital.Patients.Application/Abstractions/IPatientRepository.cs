using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.Application.Abstractions;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(
        PatientId id,
        CancellationToken cancellationToken = default);

    Task<Patient?> GetByExternalIdAsync(
        string sourceSystem,
        string externalId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Patient>> SearchAsync(
        string? name,
        string? sourceSystem,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Patient patient,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Patient patient,
        CancellationToken cancellationToken = default);
}