using Hospital.Admissions.Domain.Admissions;

namespace Hospital.Admissions.Application.Admissions.Abstractions;

public interface IAdmissionRepository
{
    Task<Admission?> GetByIdAsync(
        AdmissionId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Admission admission,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Admission admission,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Admission>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<int> CountActiveAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Admission>> SearchAsync(
        AdmissionStatus? status,
        string? unit,
        CancellationToken cancellationToken = default);
}