using Hospital.Prescriptions.Application.Abstractions;

namespace Hospital.Prescriptions.Application.SearchPrescriptions;

public sealed class SearchPrescriptionsHandler
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public SearchPrescriptionsHandler(
        IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<IReadOnlyCollection<PrescriptionResponse>> HandleAsync(
        SearchPrescriptionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var prescriptions =
            await _prescriptionRepository.SearchAsync(
                query.Status,
                cancellationToken);

        return prescriptions
            .Select(x => new PrescriptionResponse(
                x.Id.Value,
                x.PatientId,
                x.Description,
                x.PrescribedAtUtc,
                x.Status.ToString()))
            .ToList()
            .AsReadOnly();
    }
}
