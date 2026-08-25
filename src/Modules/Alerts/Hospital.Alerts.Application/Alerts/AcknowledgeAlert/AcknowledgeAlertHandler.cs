using Hospital.Alerts.Application.Alerts.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.Alerts.Application.Alerts.AcknowledgeAlert;

public sealed class AcknowledgeAlertHandler
{
    private readonly IPatientAlertRepository _repository;

    public AcknowledgeAlertHandler(
        IPatientAlertRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> HandleAsync(
        AcknowledgeAlertCommand command,
        CancellationToken cancellationToken = default)
    {
        var alert = await _repository.GetByIdAsync(
            command.AlertId,
            cancellationToken);

        if (alert is null)
        {
            return Result.Failure(
                new Error(
                    "Alert.NotFound",
                    "Alert was not found."));
        }

        alert.Acknowledge(command.AcknowledgedAtUtc);

        await _repository.UpdateAsync(
            alert,
            cancellationToken);

        return Result.Success();
    }
}