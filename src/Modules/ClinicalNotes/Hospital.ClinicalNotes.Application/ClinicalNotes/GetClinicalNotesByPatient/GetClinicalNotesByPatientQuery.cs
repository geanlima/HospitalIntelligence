namespace Hospital.ClinicalNotes.Application.ClinicalNotes.GetClinicalNotesByPatient;

public sealed record GetClinicalNotesByPatientQuery(
    Guid PatientId);