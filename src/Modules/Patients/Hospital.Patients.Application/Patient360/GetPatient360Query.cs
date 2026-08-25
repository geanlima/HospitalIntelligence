using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.Application.Patient360;

public sealed record GetPatient360Query(
    PatientId PatientId);