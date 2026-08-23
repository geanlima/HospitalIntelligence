using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.Application.Patients.GetPatientById;

public sealed record GetPatientByIdQuery(
    PatientId PatientId);