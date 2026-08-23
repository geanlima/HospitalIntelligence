namespace Hospital.Integration.Idempotency;

public interface IIdempotencyStore
{
    Task<bool> HasBeenProcessedAsync(
        Guid messageId,
        CancellationToken cancellationToken = default);

    Task MarkAsProcessedAsync(
        Guid messageId,
        CancellationToken cancellationToken = default);
}