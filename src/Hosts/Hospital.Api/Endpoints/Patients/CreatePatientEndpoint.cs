using Hospital.Api.Common;
using Hospital.Patients.Application.Patients.CreatePatient;
using Hospital.Patients.Application.Patients.Mappings;
using Hospital.Patients.Contracts.Patients;

namespace Hospital.Api.Endpoints.Patients;

public static class CreatePatientEndpoint
{
    public static IEndpointRouteBuilder MapCreatePatientEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/patients",
            async (
                CreatePatientRequest request,
                CreatePatientHandler handler,
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

                return Results.Created(
                    $"/patients/{result.Value.Value}",
                    new
                    {
                        Id = result.Value.Value
                    });
            })
            .WithName("CreatePatient")
            .WithTags("Patients")
            .WithSummary("Cria um novo paciente")
            .WithDescription(
                "Cria um paciente no HIP. " +
                "Pode receber um identificador externo opcional.")
            .Produces(
                StatusCodes.Status201Created)
            .Produces(
                StatusCodes.Status400BadRequest)
            .Produces(
                StatusCodes.Status409Conflict);

        return app;
    }
}