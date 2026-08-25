namespace Hospital.Timeline.Domain.Timeline;

public readonly record struct TimelineItemId(Guid Value)
{
    public static TimelineItemId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();
}