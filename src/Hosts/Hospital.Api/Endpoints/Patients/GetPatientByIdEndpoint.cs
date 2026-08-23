using Hospital.Api.Common;
using Hospital.Patients.Application.Patients.GetPatientById;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Api.Endpoints.Patients;

public static class GetPatientByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetPatientByIdEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/patients/{id:guid}",
            async (
                Guid id,
                GetPatientByIdHandler handler,
                CancellationToken cancellationToken) =>
            {
                var query =
                    new GetPatientByIdQuery(
                        new PatientId(id));

                var result =
                    await handler.HandleAsync(
                        query,
                        cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblem();
                }

                return Results.Ok(
                    result.Value);
            })
            .WithName("GetPatientById")
            .WithTags("Patients")
            .WithSummary("Consulta um paciente por ID")
            .WithDescription("Retorna os dados públicos do paciente pelo identificador interno do HIP.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}