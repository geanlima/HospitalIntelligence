using Hospital.Admissions.Application.Admissions.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.Admissions.Application.Admissions.DischargeAdmission;

public sealed class DischargeAdmissionHandler
{
    private readonly IAdmissionRepository _repository;

    public DischargeAdmissionHandler(
        IAdmissionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> HandleAsync(
        DischargeAdmissionCommand command,
        CancellationToken cancellationToken = default)
    {
        var admission =
            await _repository.GetByIdAsync(
                command.AdmissionId,
                cancellationToken);

        if (admission is null)
        {
            return Result.Failure(
                new Error(
                    "Admission.NotFound",
                    "Admission was not found."));
        }

        admission.Discharge(
            command.DischargeDate);

        await _repository.UpdateAsync(
            admission,
            cancellationToken);

        return Result.Success();
    }
}