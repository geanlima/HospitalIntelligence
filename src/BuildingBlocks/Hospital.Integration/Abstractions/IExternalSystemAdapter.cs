using Hospital.Integration.Messaging;

namespace Hospital.Integration.Abstractions;

public interface IExternalSystemAdapter
{
    string SourceSystem { get; }

    Task<IntegrationMessage> ReceiveAsync(
        CancellationToken cancellationToken = default);
}