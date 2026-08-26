using Hospital.VitalSigns.Domain.VitalSigns;

namespace Hospital.VitalSigns.Application.VitalSigns.Abstractions;

public interface IVitalSignRepository
{
    Task<VitalSign?> GetByIdAsync(
        VitalSignId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        VitalSign vitalSign,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<VitalSign>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<VitalSign>> SearchAsync(
        CancellationToken cancellationToken = default);
}