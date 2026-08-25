namespace Hospital.Patients.Contracts.Patient360;

public sealed record ExamSummaryResponse(
    Guid Id,
    string Name,
    DateTimeOffset RequestedAtUtc,
    DateTimeOffset? ResultedAtUtc,
    string Status);