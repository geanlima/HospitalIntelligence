using Hospital.Api.Common;
using Hospital.Prescriptions.Application.ChangePrescriptionStatus;
using Hospital.Prescriptions.Domain.Prescriptions;

namespace Hospital.Api.Endpoints.Prescriptions;

public static class ChangePrescriptionStatusEndpoint
{
    public static IEndpointRouteBuilder MapChangePrescriptionStatusEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/prescriptions/{id:guid}/status",
            async (
                Guid id,
                ChangePrescriptionStatusRequest request,
                ChangePrescriptionStatusHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new ChangePrescriptionStatusCommand(
                    new PrescriptionId(id),
                    request.Status);

                var result = await handler.HandleAsync(
                    command,
                    cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblem();
                }

                return Results.NoContent();
            })
            .WithName("ChangePrescriptionStatus")
            .WithTags("Prescriptions")
            .WithSummary("Altera o status de uma prescrição")
            .WithDescription(
                "Permite suspender, reativar, concluir ou cancelar uma prescrição.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}

public sealed record ChangePrescriptionStatusRequest(
    PrescriptionStatus Status);