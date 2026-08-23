namespace Hospital.Integration.Messaging;

public sealed record MessageEnvelope<T>
{
    public Guid MessageId { get; init; }

    public Guid CorrelationId { get; init; }

    public string SourceSystem { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public T Payload { get; init; }

    private MessageEnvelope(
        Guid messageId,
        Guid correlationId,
        string sourceSystem,
        DateTimeOffset createdAtUtc,
        T payload)
    {
        MessageId = messageId;
        CorrelationId = correlationId;
        SourceSystem = sourceSystem;
        CreatedAtUtc = createdAtUtc;
        Payload = payload;
    }

    public static MessageEnvelope<T> Create(
        string sourceSystem,
        T payload,
        Guid? correlationId = null)
    {
        if (string.IsNullOrWhiteSpace(sourceSystem))
        {
            throw new ArgumentException(
                "Source system is required.",
                nameof(sourceSystem));
        }

        ArgumentNullException.ThrowIfNull(payload);

        return new MessageEnvelope<T>(
            Guid.NewGuid(),
            correlationId ?? Guid.NewGuid(),
            sourceSystem.Trim(),
            DateTimeOffset.UtcNow,
            payload);
    }
}