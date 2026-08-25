namespace Hospital.Admissions.Application.Admissions.GetAdmissionsByPatient;

public sealed record GetAdmissionsByPatientQuery(
    Guid PatientId);