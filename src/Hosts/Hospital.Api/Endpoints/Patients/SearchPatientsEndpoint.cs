using Hospital.Patients.Application.Patients.SearchPatients;

namespace Hospital.Api.Endpoints.Patients;

public static class SearchPatientsEndpoint
{
    public static IEndpointRouteBuilder MapSearchPatientsEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/patients",
            async (
                string? name,
                SearchPatientsHandler handler,
                CancellationToken cancellationToken) =>
            {
                var query =
                    new SearchPatientsQuery(
                        name);

                var result =
                    await handler.HandleAsync(
                        query,
                        cancellationToken);

                return Results.Ok(
                    result.Value);
            })
            .WithName("SearchPatients")
            .WithTags("Patients")
            .WithSummary("Pesquisa pacientes")
            .WithDescription(
                "Pesquisa pacientes por nome. " +
                "Quando o nome não é informado, retorna todos os pacientes.")
            .Produces(
                StatusCodes.Status200OK);

        return app;
    }
}