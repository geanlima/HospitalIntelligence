using Hospital.SharedKernel.Domain;

namespace Hospital.Timeline.Domain.Timeline;

public sealed class TimelineItem
    : AggregateRoot<TimelineItemId>
{
    private TimelineItem()
        : base(default)
    {
    }

    private TimelineItem(
        TimelineItemId id,
        Guid patientId,
        DateTimeOffset occurredAtUtc,
        string type,
        string title,
        string description)
        : base(id)
    {
        PatientId = patientId;
        OccurredAtUtc = occurredAtUtc;
        Type = type;
        Title = title;
        Description = description;
    }

    public Guid PatientId { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public static TimelineItem Create(
        Guid patientId,
        DateTimeOffset occurredAtUtc,
        string type,
        string title,
        string description)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException(
                "Patient id is required.",
                nameof(patientId));
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException(
                "Timeline item type is required.",
                nameof(type));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Timeline item title is required.",
                nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Timeline item description is required.",
                nameof(description));
        }

        return new TimelineItem(
            TimelineItemId.New(),
            patientId,
            occurredAtUtc,
            type.Trim(),
            title.Trim(),
            description.Trim());
    }
}