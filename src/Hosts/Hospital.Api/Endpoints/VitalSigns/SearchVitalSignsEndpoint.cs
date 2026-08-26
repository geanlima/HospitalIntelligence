using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;
using Hospital.VitalSigns.Application.VitalSigns.SearchVitalSigns;

namespace Hospital.Api.Endpoints.VitalSigns;

public static class SearchVitalSignsEndpoint
{
    public static IEndpointRouteBuilder MapSearchVitalSignsEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/vital-signs",
                async (
                    SearchVitalSignsHandler handler,
                    IPatientRepository patientRepository,
                    CancellationToken cancellationToken) =>
                {
                    var query = new SearchVitalSignsQuery();

                    var vitalSigns =
                        await handler.HandleAsync(
                            query,
                            cancellationToken);

                    var response =
                        new List<VitalSignListResponse>();

                    foreach (var vitalSign in vitalSigns)
                    {
                        var patient =
                            await patientRepository.GetByIdAsync(
                                new PatientId(vitalSign.PatientId),
                                cancellationToken);

                        response.Add(
                            new VitalSignListResponse(
                                vitalSign.Id,
                                vitalSign.PatientId,
                                patient?.Name ?? "Paciente não encontrado",
                                vitalSign.MeasuredAtUtc,
                                vitalSign.Temperature,
                                vitalSign.HeartRate,
                                vitalSign.RespiratoryRate,
                                vitalSign.SystolicBloodPressure,
                                vitalSign.DiastolicBloodPressure,
                                vitalSign.OxygenSaturation));
                    }

                    return Results.Ok(response);
                })
            .WithTags("VitalSigns")
            .WithName("SearchVitalSigns")
            .WithSummary("Lista sinais vitais")
            .WithDescription(
                "Lista sinais vitais ordenados pela medição mais recente.");

        return app;
    }
}
