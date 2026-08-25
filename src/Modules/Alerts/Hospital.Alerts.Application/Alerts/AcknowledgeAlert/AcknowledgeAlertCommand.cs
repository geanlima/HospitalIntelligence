using Hospital.Alerts.Domain.Alerts;

namespace Hospital.Alerts.Application.Alerts.AcknowledgeAlert;

public sealed record AcknowledgeAlertCommand(
    PatientAlertId AlertId,
    DateTimeOffset AcknowledgedAtUtc);