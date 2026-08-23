using Hospital.Api.Common;
using Hospital.Patients.Application.Patients.Mappings;
using Hospital.Patients.Application.Patients.SynchronizeExternalPatient;
using Hospital.Patients.Contracts.Patients;

namespace Hospital.Api.Endpoints.Patients;

public static class SynchronizeExternalPatientEndpoint
{
    public static IEndpointRouteBuilder MapSynchronizeExternalPatientEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/patients/synchronize",
            async (
                SynchronizeExternalPatientRequest request,
                SynchronizeExternalPatientHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command =
                    request.ToCommand();

                var result =
                    await handler.HandleAsync(
                        command,
                        cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblem();
                }

                return Results.Ok(
                    new
                    {
                        Id = result.Value.Value
                    });
            })
            .WithName("SynchronizeExternalPatient")
            .WithTags("Patients")
            .WithSummary("Sincroniza paciente externo")
            .WithDescription(
                "Cria ou atualiza um paciente usando o identificador do sistema de origem.")
            .Produces(
                StatusCodes.Status200OK)
            .Produces(
                StatusCodes.Status400BadRequest);

        return app;
    }
}