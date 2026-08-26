using Hospital.Admissions.Application.Admissions.Abstractions;

namespace Hospital.Admissions.Application.Admissions.SearchAdmissions;

public sealed class SearchAdmissionsHandler
{
    private readonly IAdmissionRepository _admissionRepository;

    public SearchAdmissionsHandler(
        IAdmissionRepository admissionRepository)
    {
        _admissionRepository = admissionRepository;
    }

    public async Task<IReadOnlyCollection<AdmissionResponse>> HandleAsync(
        SearchAdmissionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var admissions =
            await _admissionRepository.SearchAsync(
                query.Status,
                query.Unit,
                cancellationToken);

        return admissions
            .Select(x => new AdmissionResponse(
                x.Id.Value,
                x.PatientId,
                x.AdmissionDate,
                x.DischargeDate,
                x.Unit,
                x.Bed,
                x.Status.ToString()))
            .ToList()
            .AsReadOnly();
    }
}