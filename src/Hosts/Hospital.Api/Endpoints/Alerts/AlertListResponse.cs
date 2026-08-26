namespace Hospital.Api.Endpoints.Alerts;

public sealed record AlertListResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    string Type,
    string Severity,
    string Description,
    DateTimeOffset CreatedAtUtc,
    string Status);
