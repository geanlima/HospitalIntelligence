using System.Text.Json;
using Hospital.Integration.Messaging;
using Hospital.MockHospital.Contracts;

namespace Hospital.MockHospital.Mappers;

public sealed class MockHospitalPatientMapper
{
    public IntegrationMessage Map(
        MockHospitalPatientMessage externalPatient,
        Guid? messageId = null,
        Guid? correlationId = null)
    {
        ArgumentNullException.ThrowIfNull(externalPatient);

        var payload =
            JsonSerializer.Serialize(externalPatient);

        return new IntegrationMessage(
            messageId ?? Guid.NewGuid(),
            correlationId ?? Guid.NewGuid(),
            "MOCK_HOSPITAL",
            "Patient",
            payload,
            DateTimeOffset.UtcNow);
    }
}