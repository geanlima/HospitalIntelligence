using Hospital.Api.Common;
using Hospital.Patients.Application.Patient360;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Api.Endpoints.Patients;

public static class GetPatient360Endpoint
{
    public static IEndpointRouteBuilder MapGetPatient360Endpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/patients/{id:guid}/360",
            async (
                Guid id,
                GetPatient360Handler handler,
                CancellationToken cancellationToken) =>
            {
                var query =
                    new GetPatient360Query(
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
            .WithName("GetPatient360")
            .WithTags("Patients")
            .WithSummary("Retorna a visão 360 do paciente")
            .WithDescription(
                "Retorna os dados consolidados do paciente, incluindo cadastro, internações, exames, prescrições, sinais vitais, evoluções, alertas e timeline clínica.")
            .Produces(
                StatusCodes.Status200OK)
            .Produces(
                StatusCodes.Status404NotFound);

        return app;
    }
}