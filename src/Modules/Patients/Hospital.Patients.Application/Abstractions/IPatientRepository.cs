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

    Task AddAsync(
        Patient patient,
        CancellationToken cancellationToken = default);
}