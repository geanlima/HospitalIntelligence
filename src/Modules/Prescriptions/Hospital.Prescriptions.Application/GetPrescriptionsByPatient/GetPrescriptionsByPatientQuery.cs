namespace Hospital.Prescriptions.Application.GetPrescriptionsByPatient;

public sealed record GetPrescriptionsByPatientQuery(
    Guid PatientId);