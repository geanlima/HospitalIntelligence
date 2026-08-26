using Hospital.Api.CommandCenter;

namespace Hospital.Api.Endpoints.CommandCenter;

public static class CommandCenterSummaryEndpoint
{
    public static IEndpointRouteBuilder MapCommandCenterSummaryEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/command-center/summary",
                async (
                    GetCommandCenterSummaryHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var summary =
                        await handler.HandleAsync(cancellationToken);

                    return Results.Ok(summary);
                })
            .WithTags("CommandCenter")
            .WithName("GetCommandCenterSummary")
            .WithSummary("Hospital Command Center — visão operacional + ML")
            .Produces<CommandCenterSummaryResponse>(StatusCodes.Status200OK);

        return app;
    }
}
