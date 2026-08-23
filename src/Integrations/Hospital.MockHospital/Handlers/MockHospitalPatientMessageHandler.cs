using System.Text.Json;
using Hospital.Integration.Abstractions;
using Hospital.Integration.Messaging;
using Hospital.MockHospital.Contracts;
using Hospital.Patients.Application.Patients.SynchronizeExternalPatient;
using Hospital.Patients.Domain.Patients;

namespace Hospital.MockHospital.Handlers;

public sealed class MockHospitalPatientMessageHandler
    : IIntegrationMessageHandler
{
    private readonly SynchronizeExternalPatientHandler _handler;

    public MockHospitalPatientMessageHandler(
        SynchronizeExternalPatientHandler handler)
    {
        _handler = handler;
    }

    public async Task HandleAsync(
        IntegrationMessage message,
        CancellationToken cancellationToken = default)
    {
        if (message.SourceSystem != "MOCK_HOSPITAL")
        {
            throw new InvalidOperationException(
                $"Unsupported source system: {message.SourceSystem}.");
        }

        if (message.MessageType != "Patient")
        {
            throw new InvalidOperationException(
                $"Unsupported message type: {message.MessageType}.");
        }

        var externalPatient =
            JsonSerializer.Deserialize<
                MockHospitalPatientMessage>(
                message.Payload);

        if (externalPatient is null)
        {
            throw new InvalidOperationException(
                "Unable to deserialize patient message.");
        }

        if (!Enum.IsDefined(
                typeof(Gender),
                externalPatient.Gender))
        {
            throw new InvalidOperationException(
                $"Invalid gender value: {externalPatient.Gender}.");
        }

        var gender =
            (Gender)externalPatient.Gender;

        var command =
            new SynchronizeExternalPatientCommand(
                message.SourceSystem,
                externalPatient.ExternalId,
                externalPatient.Name,
                externalPatient.BirthDate,
                gender);

        var result =
            await _handler.HandleAsync(
                command,
                cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"{result.Error.Code}: {result.Error.Description}");
        }
    }
}