using Hospital.Alerts.Application.Alerts.Abstractions;
using Hospital.Alerts.Domain.Alerts;
using Hospital.SharedKernel.Application;

namespace Hospital.Alerts.Application.Alerts.CreateAlert;

public sealed class CreateAlertHandler
{
    private readonly IPatientAlertRepository _repository;

    public CreateAlertHandler(
        IPatientAlertRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PatientAlertId>> HandleAsync(
        CreateAlertCommand command,
        CancellationToken cancellationToken = default)
    {
        var alert = PatientAlert.Create(
            command.PatientId,
            command.Type,
            command.Severity,
            command.Description,
            command.CreatedAtUtc);

        await _repository.AddAsync(
            alert,
            cancellationToken);

        return Result<PatientAlertId>.Success(alert.Id);
    }
}