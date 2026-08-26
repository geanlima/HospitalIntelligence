using Hospital.Alerts.Domain.Alerts;

namespace Hospital.Alerts.Application.Alerts.Abstractions;

public interface IPatientAlertRepository
{
    Task<PatientAlert?> GetByIdAsync(
        PatientAlertId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PatientAlert alert,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        PatientAlert alert,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PatientAlert>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<int> CountCriticalAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PatientAlert>> SearchAsync(
        AlertStatus? status,
        AlertSeverity? severity,
        CancellationToken cancellationToken = default);
}