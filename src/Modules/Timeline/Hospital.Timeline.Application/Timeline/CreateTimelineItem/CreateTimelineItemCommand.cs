namespace Hospital.Timeline.Application.Timeline.CreateTimelineItem;

public sealed record CreateTimelineItemCommand(
    Guid PatientId,
    DateTimeOffset OccurredAtUtc,
    string Type,
    string Title,
    string Description);