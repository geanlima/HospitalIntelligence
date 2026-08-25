using Hospital.Prescriptions.Application.Abstractions;
using Hospital.Prescriptions.Domain.Prescriptions;
using Hospital.SharedKernel.Application;

namespace Hospital.Prescriptions.Application.CreatePrescription;

public sealed class CreatePrescriptionHandler
{
    private readonly IPrescriptionRepository _repository;

    public CreatePrescriptionHandler(
        IPrescriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PrescriptionId>> HandleAsync(
        CreatePrescriptionCommand command,
        CancellationToken cancellationToken = default)
    {
        var prescription =
            Prescription.Create(
                command.PatientId,
                command.Description,
                command.PrescribedAtUtc);

        await _repository.AddAsync(
            prescription,
            cancellationToken);

        return Result<PrescriptionId>.Success(
            prescription.Id);
    }
}