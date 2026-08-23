using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.Application.Patients.CreatePatient;

public sealed record CreatePatientCommand(
    string Name,
    DateOnly BirthDate,
    Gender Gender,
    string? SourceSystem = null,
    string? ExternalId = null);