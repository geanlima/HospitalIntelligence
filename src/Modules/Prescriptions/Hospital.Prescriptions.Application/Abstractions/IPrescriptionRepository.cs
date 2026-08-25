using Hospital.Prescriptions.Domain.Prescriptions;

namespace Hospital.Prescriptions.Application.Abstractions;

public interface IPrescriptionRepository
{
    Task<Prescription?> GetByIdAsync(
        PrescriptionId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Prescription prescription,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Prescription prescription,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Prescription>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}