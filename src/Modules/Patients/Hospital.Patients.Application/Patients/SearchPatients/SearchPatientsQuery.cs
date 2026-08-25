namespace Hospital.Patients.Application.Patients.SearchPatients;

public sealed record SearchPatientsQuery(
    string? Name,
    string? SourceSystem);