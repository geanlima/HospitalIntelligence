using Hospital.Integration.Messaging;

namespace Hospital.Integration.Abstractions;

public interface IIntegrationMessageHandler
{
    Task HandleAsync(
        IntegrationMessage message,
        CancellationToken cancellationToken = default);
}