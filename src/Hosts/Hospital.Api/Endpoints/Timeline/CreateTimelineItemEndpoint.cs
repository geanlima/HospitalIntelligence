using Hospital.Timeline.Application.Timeline.CreateTimelineItem;

namespace Hospital.Api.Endpoints.Timeline;

public static class CreateTimelineItemEndpoint
{
    public static IEndpointRouteBuilder MapCreateTimelineItemEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/timeline",
            async (
                CreateTimelineItemRequest request,
                CreateTimelineItemHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateTimelineItemCommand(
                    request.PatientId,
                    request.OccurredAtUtc,
                    request.Type,
                    request.Title,
                    request.Description);

                var result = await handler.HandleAsync(
                    command,
                    cancellationToken);

                if (result.IsFailure)
                {
                    return Results.BadRequest(new
                    {
                        result.Error.Code,
                        result.Error.Description
                    });
                }

                return Results.Created(
                    $"/timeline/{result.Value.Value}",
                    new
                    {
                        Id = result.Value.Value
                    });
            })
            .WithName("CreateTimelineItem")
            .WithTags("Timeline")
            .WithSummary("Registra um evento na timeline do paciente")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}

public sealed record CreateTimelineItemRequest(
    Guid PatientId,
    DateTimeOffset OccurredAtUtc,
    string Type,
    string Title,
    string Description);