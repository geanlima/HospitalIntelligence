using Hospital.ClinicalNotes.Domain.ClinicalNotes;

namespace Hospital.ClinicalNotes.Application.ClinicalNotes.SearchClinicalNotes;

public sealed record SearchClinicalNotesQuery(
    ClinicalNoteType? NoteType);
