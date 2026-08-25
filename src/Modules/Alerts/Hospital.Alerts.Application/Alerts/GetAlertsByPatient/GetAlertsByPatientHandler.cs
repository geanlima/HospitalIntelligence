using Hospital.Alerts.Application.Alerts.Abstractions;
using Hospital.Alerts.Domain.Alerts;

namespace Hospital.Alerts.Application.Alerts.GetAlertsByPatient;

public sealed class GetAlertsByPatientHandler
{
    private readonly IPatientAlertRepository _repository;

    public GetAlertsByPatientHandler(
        IPatientAlertRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<PatientAlert>> HandleAsync(
        GetAlertsByPatientQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByPatientIdAsync(
            query.PatientId,
            cancellationToken);
    }
}