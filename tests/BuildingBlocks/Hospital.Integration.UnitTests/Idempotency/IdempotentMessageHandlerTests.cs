using Hospital.Integration.Abstractions;
using Hospital.Integration.Idempotency;
using Hospital.Integration.Messaging;

namespace Hospital.Integration.UnitTests.Idempotency;

public sealed class IdempotentMessageHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenMessageWasNotProcessed_ShouldProcessAndMarkAsProcessed()
    {
        var message =
            new IntegrationMessage(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "TEST",
                "PatientCreated",
                "{}",
                DateTimeOffset.UtcNow);

        var innerHandler =
            new FakeIntegrationMessageHandler();

        var idempotencyStore =
            new FakeIdempotencyStore();

        var handler =
            new IdempotentMessageHandler(
                innerHandler,
                idempotencyStore);

        await handler.HandleAsync(message);

        Assert.Equal(
            1,
            innerHandler.HandleCount);

        var wasProcessed =
            await idempotencyStore.HasBeenProcessedAsync(
                message.MessageId);

        Assert.True(wasProcessed);
    }

    [Fact]
    public async Task HandleAsync_WhenMessageWasAlreadyProcessed_ShouldNotProcessAgain()
    {
        var message =
            new IntegrationMessage(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "TEST",
                "PatientCreated",
                "{}",
                DateTimeOffset.UtcNow);

        var innerHandler =
            new FakeIntegrationMessageHandler();

        var idempotencyStore =
            new FakeIdempotencyStore();

        await idempotencyStore.MarkAsProcessedAsync(
            message.MessageId);

        var handler =
            new IdempotentMessageHandler(
                innerHandler,
                idempotencyStore);

        await handler.HandleAsync(message);

        Assert.Equal(
            0,
            innerHandler.HandleCount);
    }

    [Fact]
    public async Task HandleAsync_WhenInnerHandlerFails_ShouldNotMarkMessageAsProcessed()
    {
        var message =
            new IntegrationMessage(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "TEST",
                "PatientCreated",
                "{}",
                DateTimeOffset.UtcNow);

        var innerHandler =
            new FailingIntegrationMessageHandler();

        var idempotencyStore =
            new FakeIdempotencyStore();

        var handler =
            new IdempotentMessageHandler(
                innerHandler,
                idempotencyStore);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(message));

        var wasProcessed =
            await idempotencyStore.HasBeenProcessedAsync(
                message.MessageId);

        Assert.False(wasProcessed);
    }

    private sealed class FakeIntegrationMessageHandler
        : IIntegrationMessageHandler
    {
        public int HandleCount { get; private set; }

        public Task HandleAsync(
            IntegrationMessage message,
            CancellationToken cancellationToken = default)
        {
            HandleCount++;

            return Task.CompletedTask;
        }
    }

    private sealed class FailingIntegrationMessageHandler
        : IIntegrationMessageHandler
    {
        public Task HandleAsync(
            IntegrationMessage message,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException(
                "Simulated integration failure.");
        }
    }

    private sealed class FakeIdempotencyStore
        : IIdempotencyStore
    {
        private readonly HashSet<Guid> _processedMessages = [];

        public Task<bool> HasBeenProcessedAsync(
            Guid messageId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _processedMessages.Contains(messageId));
        }

        public Task MarkAsProcessedAsync(
            Guid messageId,
            CancellationToken cancellationToken = default)
        {
            _processedMessages.Add(messageId);

            return Task.CompletedTask;
        }
    }
}   