using Hospital.Admissions.Application.Admissions.SearchAdmissions;
using Hospital.Admissions.Domain.Admissions;
using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Api.Endpoints.Admissions;

public static class SearchAdmissionsEndpoint
{
    public static IEndpointRouteBuilder MapSearchAdmissionsEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/admissions",
                async (
                    AdmissionStatus? status,
                    string? unit,
                    SearchAdmissionsHandler handler,
                    IPatientRepository patientRepository,
                    CancellationToken cancellationToken) =>
                {
                    var query = new SearchAdmissionsQuery(
                        status,
                        unit);

                    var admissions =
                        await handler.HandleAsync(
                            query,
                            cancellationToken);

                    var response =
                        new List<AdmissionListResponse>();

                    foreach (var admission in admissions)
                    {
                        var patient =
                            await patientRepository.GetByIdAsync(
                                new PatientId(admission.PatientId),
                                cancellationToken);

                        response.Add(
                            new AdmissionListResponse(
                                admission.Id,
                                admission.PatientId,
                                patient?.Name ?? "Paciente não encontrado",
                                admission.AdmissionDate,
                                admission.DischargeDate,
                                admission.Unit,
                                admission.Bed,
                                admission.Status));
                    }

                    return Results.Ok(response);
                })
            .WithTags("Admissions")
            .WithName("SearchAdmissions")
            .WithSummary("Lista internações")
            .WithDescription(
                "Lista internações com filtros opcionais por status e unidade.");

        return app;
    }
}