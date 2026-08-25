namespace Hospital.Patients.Contracts.Patient360;

public sealed record PrescriptionSummaryResponse(
    Guid Id,
    string Description,
    DateTimeOffset PrescribedAtUtc,
    string Status);