using Hospital.Api.Common;
using Hospital.Patients.Application.Patients.Mappings;
using Hospital.Patients.Application.Patients.UpdatePatient;
using Hospital.Patients.Contracts.Patients;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Api.Endpoints.Patients;

public static class UpdatePatientEndpoint
{
    public static IEndpointRouteBuilder MapUpdatePatientEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/patients/{id:guid}",
            async (
                Guid id,
                UpdatePatientRequest request,
                UpdatePatientHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command =
                    request.ToCommand(
                        new PatientId(id));

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
            .WithName("UpdatePatient")
            .WithTags("Patients")
            .WithSummary("Atualiza um paciente")
            .WithDescription(
                "Atualiza os dados cadastrais de um paciente existente.")
            .Produces(
                StatusCodes.Status204NoContent)
            .Produces(
                StatusCodes.Status400BadRequest)
            .Produces(
                StatusCodes.Status404NotFound);

        return app;
    }
}