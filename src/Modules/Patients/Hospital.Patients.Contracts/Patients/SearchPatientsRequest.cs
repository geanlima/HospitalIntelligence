namespace Hospital.Patients.Contracts.Patients;

public sealed record SearchPatientsRequest(
    string? Name,
    string? SourceSystem);