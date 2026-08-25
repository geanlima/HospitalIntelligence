namespace Hospital.Patients.Contracts.Patient360;

public sealed record PatientTimelineItemResponse(
    Guid Id,
    DateTimeOffset OccurredAtUtc,
    string Type,
    string Title,
    string Description);