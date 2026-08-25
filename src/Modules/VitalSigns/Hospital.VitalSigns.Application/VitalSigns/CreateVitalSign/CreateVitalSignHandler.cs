using Hospital.SharedKernel.Application;
using Hospital.VitalSigns.Application.VitalSigns.Abstractions;
using Hospital.VitalSigns.Domain.VitalSigns;

namespace Hospital.VitalSigns.Application.VitalSigns.CreateVitalSign;

public sealed class CreateVitalSignHandler
{
    private readonly IVitalSignRepository _repository;

    public CreateVitalSignHandler(
        IVitalSignRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<VitalSignId>> HandleAsync(
        CreateVitalSignCommand command,
        CancellationToken cancellationToken = default)
    {
        var vitalSign = VitalSign.Create(
            command.PatientId,
            command.MeasuredAtUtc,
            command.Temperature,
            command.HeartRate,
            command.RespiratoryRate,
            command.SystolicBloodPressure,
            command.DiastolicBloodPressure,
            command.OxygenSaturation);

        await _repository.AddAsync(
            vitalSign,
            cancellationToken);

        return Result<VitalSignId>.Success(
            vitalSign.Id);
    }
}