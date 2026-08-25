namespace Hospital.ClinicalNotes.Domain.ClinicalNotes;

public readonly record struct ClinicalNoteId(Guid Value)
{
    public static ClinicalNoteId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();
}