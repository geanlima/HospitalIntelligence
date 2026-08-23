namespace Hospital.Patients.Contracts.Patients;

public sealed record CreatePatientRequest(
    string Name,
    DateOnly BirthDate,
    int Gender,
    string? SourceSystem,
    string? ExternalId);