using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.Application.Patients.UpdatePatient;

public sealed record UpdatePatientCommand(
    PatientId PatientId,
    string Name,
    DateOnly BirthDate,
    Gender Gender);