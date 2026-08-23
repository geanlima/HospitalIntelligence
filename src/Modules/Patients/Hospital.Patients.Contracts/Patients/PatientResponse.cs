namespace Hospital.Patients.Contracts.Patients;

public sealed record PatientResponse(
    Guid Id,
    string Name,
    DateOnly BirthDate,
    string Gender,
    string? SourceSystem,
    string? ExternalId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);