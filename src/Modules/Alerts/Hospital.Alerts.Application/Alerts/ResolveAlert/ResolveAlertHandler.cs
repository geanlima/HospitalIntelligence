using Hospital.Alerts.Application.Alerts.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.Alerts.Application.Alerts.ResolveAlert;

public sealed class ResolveAlertHandler
{
    private readonly IPatientAlertRepository _repository;

    public ResolveAlertHandler(
        IPatientAlertRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> HandleAsync(
        ResolveAlertCommand command,
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

        alert.Resolve(command.ResolvedAtUtc);

        await _repository.UpdateAsync(
            alert,
            cancellationToken);

        return Result.Success();
    }
}