using Hospital.Integration.Messaging;

namespace Hospital.Integration.Abstractions;

public interface IExternalMessageMapper<in TExternal>
{
    IntegrationMessage Map(
        TExternal externalMessage);
}