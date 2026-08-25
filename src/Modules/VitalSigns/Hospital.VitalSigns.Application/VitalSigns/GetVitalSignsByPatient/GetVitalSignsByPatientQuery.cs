namespace Hospital.VitalSigns.Application.VitalSigns.GetVitalSignsByPatient;

public sealed record GetVitalSignsByPatientQuery(
    Guid PatientId);