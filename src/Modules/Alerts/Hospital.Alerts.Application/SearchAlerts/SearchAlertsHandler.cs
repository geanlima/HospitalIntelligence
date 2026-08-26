using Hospital.Alerts.Application.Alerts.Abstractions;

namespace Hospital.Alerts.Application.Alerts.SearchAlerts;

public sealed class SearchAlertsHandler
{
    private readonly IPatientAlertRepository _alertRepository;

    public SearchAlertsHandler(
        IPatientAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<IReadOnlyCollection<AlertResponse>> HandleAsync(
        SearchAlertsQuery query,
        CancellationToken cancellationToken = default)
    {
        var alerts =
            await _alertRepository.SearchAsync(
                query.Status,
                query.Severity,
                cancellationToken);

        return alerts
            .Select(x => new AlertResponse(
                x.Id.Value,
                x.PatientId,
                x.Type,
                x.Severity.ToString(),
                x.Description,
                x.CreatedAtUtc,
                x.Status.ToString()))
            .ToList()
            .AsReadOnly();
    }
}
