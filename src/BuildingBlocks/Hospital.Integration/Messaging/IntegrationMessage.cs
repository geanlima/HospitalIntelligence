namespace Hospital.Integration.Messaging;

public sealed record IntegrationMessage(
    Guid MessageId,
    Guid CorrelationId,
    string SourceSystem,
    string MessageType,
    string Payload,
    DateTimeOffset ReceivedAtUtc);