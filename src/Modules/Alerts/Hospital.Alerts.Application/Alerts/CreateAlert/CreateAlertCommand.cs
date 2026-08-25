using Hospital.Alerts.Domain.Alerts;

namespace Hospital.Alerts.Application.Alerts.CreateAlert;

public sealed record CreateAlertCommand(
    Guid PatientId,
    string Type,
    AlertSeverity Severity,
    string Description,
    DateTimeOffset CreatedAtUtc);