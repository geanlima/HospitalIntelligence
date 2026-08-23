namespace Hospital.Integration.Events;

public abstract record IntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTimeOffset OccurredOnUtc { get; init; } =
        DateTimeOffset.UtcNow;
}