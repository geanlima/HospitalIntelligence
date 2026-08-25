using Hospital.Prescriptions.Application.Abstractions;
using Hospital.Prescriptions.Domain.Prescriptions;
using Hospital.SharedKernel.Application;

namespace Hospital.Prescriptions.Application.ChangePrescriptionStatus;

public sealed class ChangePrescriptionStatusHandler
{
    private readonly IPrescriptionRepository _repository;

    public ChangePrescriptionStatusHandler(
        IPrescriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> HandleAsync(
        ChangePrescriptionStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var prescription =
            await _repository.GetByIdAsync(
                command.PrescriptionId,
                cancellationToken);

        if (prescription is null)
        {
            return Result.Failure(
                new Error(
                    "Prescription.NotFound",
                    "Prescription was not found."));
        }

        switch (command.Status)
        {
            case PrescriptionStatus.Active:
                prescription.Reactivate();
                break;

            case PrescriptionStatus.Suspended:
                prescription.Suspend();
                break;

            case PrescriptionStatus.Completed:
                prescription.Complete();
                break;

            case PrescriptionStatus.Cancelled:
                prescription.Cancel();
                break;

            default:
                return Result.Failure(
                    new Error(
                        "Prescription.Status.Invalid",
                        "Prescription status is invalid."));
        }

        await _repository.UpdateAsync(
            prescription,
            cancellationToken);

        return Result.Success();
    }
}