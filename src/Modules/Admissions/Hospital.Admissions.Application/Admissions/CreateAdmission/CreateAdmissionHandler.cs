using Hospital.Admissions.Application.Admissions.Abstractions;
using Hospital.Admissions.Domain.Admissions;
using Hospital.SharedKernel.Application;

namespace Hospital.Admissions.Application.Admissions.CreateAdmission;

public sealed class CreateAdmissionHandler
{
    private readonly IAdmissionRepository _repository;

    public CreateAdmissionHandler(
        IAdmissionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<AdmissionId>> HandleAsync(
        CreateAdmissionCommand command,
        CancellationToken cancellationToken = default)
    {
        var admission =
            Admission.Create(
                command.PatientId,
                command.AdmissionDate,
                command.Unit,
                command.Bed);

        await _repository.AddAsync(
            admission,
            cancellationToken);

        return Result<AdmissionId>.Success(
            admission.Id);
    }
}