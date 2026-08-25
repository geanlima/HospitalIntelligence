using Hospital.Alerts.Domain.Alerts;

namespace Hospital.Alerts.Application.Alerts.ResolveAlert;

public sealed record ResolveAlertCommand(
    PatientAlertId AlertId,
    DateTimeOffset ResolvedAtUtc);