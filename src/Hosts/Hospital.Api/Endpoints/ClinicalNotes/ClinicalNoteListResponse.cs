namespace Hospital.Api.Endpoints.ClinicalNotes;

public sealed record ClinicalNoteListResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    string Professional,
    string NoteType,
    string Content,
    DateTimeOffset CreatedAtUtc);
