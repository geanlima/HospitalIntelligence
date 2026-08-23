using Hospital.Integration.Events;

namespace Hospital.Integration.Abstractions;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(
        IntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default);
}