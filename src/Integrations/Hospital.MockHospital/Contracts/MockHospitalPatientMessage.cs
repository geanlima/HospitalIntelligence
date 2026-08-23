namespace Hospital.MockHospital.Contracts;

public sealed record MockHospitalPatientMessage(
    string ExternalId,
    string Name,
    DateOnly BirthDate,
    int Gender);