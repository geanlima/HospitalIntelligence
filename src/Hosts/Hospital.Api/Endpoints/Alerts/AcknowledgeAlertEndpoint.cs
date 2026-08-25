using Hospital.Alerts.Application.Alerts.AcknowledgeAlert;
using Hospital.Alerts.Domain.Alerts;
using Hospital.Api.Common;

namespace Hospital.Api.Endpoints.Alerts;

public static class AcknowledgeAlertEndpoint
{
    public static IEndpointRouteBuilder MapAcknowledgeAlertEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/alerts/{id:guid}/acknowledge",
            async (
                Guid id,
                AcknowledgeAlertRequest request,
                AcknowledgeAlertHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new AcknowledgeAlertCommand(
                    new PatientAlertId(id),
                    request.AcknowledgedAtUtc);

                var result = await handler.HandleAsync(
                    command,
                    cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblem();
                }

                return Results.NoContent();
            })
            .WithName("AcknowledgeAlert")
            .WithTags("Alerts")
            .WithSummary("Reconhece um alerta clínico")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}

public sealed record AcknowledgeAlertRequest(
    DateTimeOffset AcknowledgedAtUtc);