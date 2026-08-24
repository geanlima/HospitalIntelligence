using System.Text.Json;
using Hospital.Integration.Abstractions;
using Hospital.Integration.Messaging;
using Hospital.Patients.Application.Patients.SynchronizeExternalPatient;
using Hospital.Patients.Domain.Patients;
using Hospital.Salux.Contracts;

namespace Hospital.Salux.Handlers;

public sealed class SaluxPatientMessageHandler
    : IIntegrationMessageHandler
{
    private readonly SynchronizeExternalPatientHandler _handler;

    public SaluxPatientMessageHandler(
        SynchronizeExternalPatientHandler handler)
    {
        _handler = handler;
    }

    public async Task HandleAsync(
        IntegrationMessage message,
        CancellationToken cancellationToken = default)
    {
        if (message.SourceSystem != "SALUX")
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
            JsonSerializer.Deserialize<SaluxPatientRecord>(
                message.Payload);

        if (externalPatient is null)
        {
            throw new InvalidOperationException(
                "Unable to deserialize Salux patient message.");
        }

        if (!Enum.IsDefined(
                typeof(Gender),
                externalPatient.GenderCode))
        {
            throw new InvalidOperationException(
                $"Invalid gender value: {externalPatient.GenderCode}.");
        }

        var gender =
            (Gender)externalPatient.GenderCode;

        var command =
            new SynchronizeExternalPatientCommand(
                message.SourceSystem,
                externalPatient.PatientCode,
                externalPatient.PatientName,
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