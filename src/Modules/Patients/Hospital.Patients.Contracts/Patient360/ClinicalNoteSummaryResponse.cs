namespace Hospital.Patients.Contracts.Patient360;

public sealed record ClinicalNoteSummaryResponse(
    Guid Id,
    DateTimeOffset CreatedAtUtc,
    string Professional,
    string NoteType,
    string Summary);