using Hospital.Admissions.Application.Admissions.DischargeAdmission;
using Hospital.Admissions.Domain.Admissions;
using Hospital.Api.Common;

namespace Hospital.Api.Endpoints.Admissions;

public static class DischargeAdmissionEndpoint
{
    public static IEndpointRouteBuilder MapDischargeAdmissionEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/admissions/{id:guid}/discharge",
            async (
                Guid id,
                DischargeAdmissionRequest request,
                DischargeAdmissionHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command =
                    new DischargeAdmissionCommand(
                        new AdmissionId(id),
                        request.DischargeDate);

                var result =
                    await handler.HandleAsync(
                        command,
                        cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblem();
                }

                return Results.NoContent();
            })
            .WithName("DischargeAdmission")
            .WithTags("Admissions")
            .WithSummary("Realiza a alta de uma internação")
            .WithDescription(
                "Finaliza uma internação ativa, registrando a data de alta.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}

public sealed record DischargeAdmissionRequest(
    DateTimeOffset DischargeDate);