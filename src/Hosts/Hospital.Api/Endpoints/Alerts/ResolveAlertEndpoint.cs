using Hospital.Alerts.Application.Alerts.ResolveAlert;
using Hospital.Alerts.Domain.Alerts;
using Hospital.Api.Common;

namespace Hospital.Api.Endpoints.Alerts;

public static class ResolveAlertEndpoint
{
    public static IEndpointRouteBuilder MapResolveAlertEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/alerts/{id:guid}/resolve",
            async (
                Guid id,
                ResolveAlertRequest request,
                ResolveAlertHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new ResolveAlertCommand(
                    new PatientAlertId(id),
                    request.ResolvedAtUtc);

                var result = await handler.HandleAsync(
                    command,
                    cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblem();
                }

                return Results.NoContent();
            })
            .WithName("ResolveAlert")
            .WithTags("Alerts")
            .WithSummary("Resolve um alerta clínico")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}

public sealed record ResolveAlertRequest(
    DateTimeOffset ResolvedAtUtc);