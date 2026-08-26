using Hospital.Dashboard.Application.Dashboard;

namespace Hospital.Api.Endpoints.Dashboard;

public static class DashboardSummaryEndpoint
{
    public static IEndpointRouteBuilder MapDashboardSummaryEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/dashboard/summary",
                async (
                    GetDashboardSummaryHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetDashboardSummaryQuery();

                    var response = await handler.HandleAsync(
                        query,
                        cancellationToken);

                    return Results.Ok(response);
                })
            .WithTags("Dashboard")
            .WithName("GetDashboardSummary")
            .WithSummary("Obtém o resumo do dashboard")
            .WithDescription(
                "Retorna os principais indicadores da operação hospitalar.");

        return app;
    }
}