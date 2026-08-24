using System.Text.Json;
using Hospital.Integration.Abstractions;
using Hospital.Integration.Messaging;
using Hospital.Salux.Contracts;

namespace Hospital.Salux.Mappers;

public sealed class SaluxPatientMapper
    : IExternalMessageMapper<SaluxPatientRecord>
{
    public IntegrationMessage Map(
        SaluxPatientRecord externalMessage)
    {
        ArgumentNullException.ThrowIfNull(externalMessage);

        var payload =
            JsonSerializer.Serialize(externalMessage);

        return new IntegrationMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "SALUX",
            "Patient",
            payload,
            DateTimeOffset.UtcNow);
    }
}