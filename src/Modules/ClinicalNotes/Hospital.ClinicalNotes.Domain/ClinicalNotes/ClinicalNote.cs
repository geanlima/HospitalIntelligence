using Hospital.SharedKernel.Domain;

namespace Hospital.ClinicalNotes.Domain.ClinicalNotes;

public sealed class ClinicalNote
    : AggregateRoot<ClinicalNoteId>
{
    private ClinicalNote()
        : base(default)
    {
    }

    private ClinicalNote(
        ClinicalNoteId id,
        Guid patientId,
        string professional,
        ClinicalNoteType noteType,
        string content,
        DateTimeOffset createdAtUtc)
        : base(id)
    {
        PatientId = patientId;
        Professional = professional;
        NoteType = noteType;
        Content = content;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid PatientId { get; private set; }

    public string Professional { get; private set; } = string.Empty;

    public ClinicalNoteType NoteType { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static ClinicalNote Create(
        Guid patientId,
        string professional,
        ClinicalNoteType noteType,
        string content,
        DateTimeOffset createdAtUtc)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException(
                "Patient id is required.",
                nameof(patientId));
        }

        if (string.IsNullOrWhiteSpace(professional))
        {
            throw new ArgumentException(
                "Professional is required.",
                nameof(professional));
        }

        if (!Enum.IsDefined(noteType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(noteType),
                "Clinical note type is invalid.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException(
                "Clinical note content is required.",
                nameof(content));
        }

        return new ClinicalNote(
            ClinicalNoteId.New(),
            patientId,
            professional.Trim(),
            noteType,
            content.Trim(),
            createdAtUtc);
    }
}