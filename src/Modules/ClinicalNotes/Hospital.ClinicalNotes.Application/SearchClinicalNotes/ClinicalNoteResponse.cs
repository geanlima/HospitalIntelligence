namespace Hospital.ClinicalNotes.Application.ClinicalNotes.SearchClinicalNotes;

public sealed record ClinicalNoteResponse(
    Guid Id,
    Guid PatientId,
    string Professional,
    string NoteType,
    string Content,
    DateTimeOffset CreatedAtUtc);
