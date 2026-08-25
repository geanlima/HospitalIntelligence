using Hospital.Alerts.Application.Alerts.CreateAlert;
using Hospital.Alerts.Domain.Alerts;

namespace Hospital.Api.Endpoints.Alerts;

public static class CreateAlertEndpoint
{
    public static IEndpointRouteBuilder MapCreateAlertEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/alerts",
            async (
                CreateAlertRequest request,
                CreateAlertHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateAlertCommand(
                    request.PatientId,
                    request.Type,
                    request.Severity,
                    request.Description,
                    request.CreatedAtUtc);

                var result = await handler.HandleAsync(
                    command,
                    cancellationToken);

                if (result.IsFailure)
                {
                    return Results.BadRequest(new
                    {
                        result.Error.Code,
                        result.Error.Description
                    });
                }

                return Results.Created(
                    $"/alerts/{result.Value.Value}",
                    new
                    {
                        Id = result.Value.Value
                    });
            })
            .WithName("CreateAlert")
            .WithTags("Alerts")
            .WithSummary("Cria um alerta clínico para o paciente")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}

public sealed record CreateAlertRequest(
    Guid PatientId,
    string Type,
    AlertSeverity Severity,
    string Description,
    DateTimeOffset CreatedAtUtc);