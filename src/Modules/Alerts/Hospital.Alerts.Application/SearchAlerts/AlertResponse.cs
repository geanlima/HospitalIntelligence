namespace Hospital.Alerts.Application.Alerts.SearchAlerts;

public sealed record AlertResponse(
    Guid Id,
    Guid PatientId,
    string Type,
    string Severity,
    string Description,
    DateTimeOffset CreatedAtUtc,
    string Status);
