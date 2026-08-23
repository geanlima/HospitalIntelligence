using Hospital.Integration.Abstractions;
using Hospital.Integration.Messaging;

namespace Hospital.Integration.Idempotency;

public sealed class IdempotentMessageHandler
    : IIntegrationMessageHandler
{
    private readonly IIntegrationMessageHandler _innerHandler;
    private readonly IIdempotencyStore _idempotencyStore;

    public IdempotentMessageHandler(
        IIntegrationMessageHandler innerHandler,
        IIdempotencyStore idempotencyStore)
    {
        _innerHandler = innerHandler;
        _idempotencyStore = idempotencyStore;
    }

    public async Task HandleAsync(
        IntegrationMessage message,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed =
            await _idempotencyStore.HasBeenProcessedAsync(
                message.MessageId,
                cancellationToken);

        if (alreadyProcessed)
        {
            return;
        }

        await _innerHandler.HandleAsync(
            message,
            cancellationToken);

        await _idempotencyStore.MarkAsProcessedAsync(
            message.MessageId,
            cancellationToken);
    }
}