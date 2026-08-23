namespace Hospital.Patients.Contracts.Patients;

public sealed record UpdatePatientRequest(
    string Name,
    DateOnly BirthDate,
    int Gender);