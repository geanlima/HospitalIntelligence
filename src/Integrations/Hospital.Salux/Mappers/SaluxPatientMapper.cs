using System.Security.Cryptography;
using System.Text;
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

        var updatedAt =
            externalMessage.UpdatedAtUtc == default
                ? DateTimeOffset.UnixEpoch
                : externalMessage.UpdatedAtUtc;

        var messageId = CreateDeterministicGuid(
            $"SALUX|Patient|{externalMessage.PatientCode}|{updatedAt:O}");

        var payload =
            JsonSerializer.Serialize(externalMessage);

        return new IntegrationMessage(
            messageId,
            Guid.NewGuid(),
            "SALUX",
            "Patient",
            payload,
            DateTimeOffset.UtcNow);
    }

    private static Guid CreateDeterministicGuid(string value)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(value));
        return new Guid(hash);
    }
}
