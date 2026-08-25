namespace Hospital.Patients.Contracts.Patient360;

public sealed record PatientAlertResponse(
    Guid Id,
    string Type,
    string Severity,
    string Description,
    DateTimeOffset CreatedAtUtc);