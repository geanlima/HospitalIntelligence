namespace Hospital.Patients.Contracts.Patients;

public sealed record SynchronizeExternalPatientRequest(
    string SourceSystem,
    string ExternalId,
    string Name,
    DateOnly BirthDate,
    int Gender);