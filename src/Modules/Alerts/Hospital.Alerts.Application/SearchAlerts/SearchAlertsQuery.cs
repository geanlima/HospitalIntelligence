using Hospital.Alerts.Domain.Alerts;

namespace Hospital.Alerts.Application.Alerts.SearchAlerts;

public sealed record SearchAlertsQuery(
    AlertStatus? Status,
    AlertSeverity? Severity);
