using Hospital.Alerts.Application.Alerts.SearchAlerts;
using Hospital.Alerts.Domain.Alerts;
using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Api.Endpoints.Alerts;

public static class SearchAlertsEndpoint
{
    public static IEndpointRouteBuilder MapSearchAlertsEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/alerts",
                async (
                    AlertStatus? status,
                    AlertSeverity? severity,
                    SearchAlertsHandler handler,
                    IPatientRepository patientRepository,
                    CancellationToken cancellationToken) =>
                {
                    var query = new SearchAlertsQuery(
                        status,
                        severity);

                    var alerts =
                        await handler.HandleAsync(
                            query,
                            cancellationToken);

                    var response =
                        new List<AlertListResponse>();

                    foreach (var alert in alerts)
                    {
                        var patient =
                            await patientRepository.GetByIdAsync(
                                new PatientId(alert.PatientId),
                                cancellationToken);

                        response.Add(
                            new AlertListResponse(
                                alert.Id,
                                alert.PatientId,
                                patient?.Name ?? "Paciente não encontrado",
                                alert.Type,
                                alert.Severity,
                                alert.Description,
                                alert.CreatedAtUtc,
                                alert.Status));
                    }

                    return Results.Ok(response);
                })
            .WithTags("Alerts")
            .WithName("SearchAlerts")
            .WithSummary("Lista alertas")
            .WithDescription(
                "Lista alertas com filtros opcionais por status e severidade.");

        return app;
    }
}
