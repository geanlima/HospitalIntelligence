using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;
using Hospital.Prescriptions.Application.SearchPrescriptions;
using Hospital.Prescriptions.Domain.Prescriptions;

namespace Hospital.Api.Endpoints.Prescriptions;

public static class SearchPrescriptionsEndpoint
{
    public static IEndpointRouteBuilder MapSearchPrescriptionsEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/prescriptions",
                async (
                    PrescriptionStatus? status,
                    SearchPrescriptionsHandler handler,
                    IPatientRepository patientRepository,
                    CancellationToken cancellationToken) =>
                {
                    var query = new SearchPrescriptionsQuery(
                        status);

                    var prescriptions =
                        await handler.HandleAsync(
                            query,
                            cancellationToken);

                    var response =
                        new List<PrescriptionListResponse>();

                    foreach (var prescription in prescriptions)
                    {
                        var patient =
                            await patientRepository.GetByIdAsync(
                                new PatientId(prescription.PatientId),
                                cancellationToken);

                        response.Add(
                            new PrescriptionListResponse(
                                prescription.Id,
                                prescription.PatientId,
                                patient?.Name ?? "Paciente não encontrado",
                                prescription.Description,
                                prescription.PrescribedAtUtc,
                                prescription.Status));
                    }

                    return Results.Ok(response);
                })
            .WithTags("Prescriptions")
            .WithName("SearchPrescriptions")
            .WithSummary("Lista prescrições")
            .WithDescription(
                "Lista prescrições com filtro opcional por status.");

        return app;
    }
}
