namespace Hospital.Exams.Application.Exams.GetExamsByPatient;

public sealed record GetExamsByPatientQuery(
    Guid PatientId);