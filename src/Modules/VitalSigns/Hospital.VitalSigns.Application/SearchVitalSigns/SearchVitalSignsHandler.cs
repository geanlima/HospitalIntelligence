using Hospital.VitalSigns.Application.VitalSigns.Abstractions;

namespace Hospital.VitalSigns.Application.VitalSigns.SearchVitalSigns;

public sealed class SearchVitalSignsHandler
{
    private readonly IVitalSignRepository _vitalSignRepository;

    public SearchVitalSignsHandler(
        IVitalSignRepository vitalSignRepository)
    {
        _vitalSignRepository = vitalSignRepository;
    }

    public async Task<IReadOnlyCollection<VitalSignResponse>> HandleAsync(
        SearchVitalSignsQuery query,
        CancellationToken cancellationToken = default)
    {
        var vitalSigns =
            await _vitalSignRepository.SearchAsync(
                cancellationToken);

        return vitalSigns
            .Select(x => new VitalSignResponse(
                x.Id.Value,
                x.PatientId,
                x.MeasuredAtUtc,
                x.Temperature,
                x.HeartRate,
                x.RespiratoryRate,
                x.SystolicBloodPressure,
                x.DiastolicBloodPressure,
                x.OxygenSaturation))
            .ToList()
            .AsReadOnly();
    }
}
