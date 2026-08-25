using Hospital.Exams.Application.Exams.CreateExam;

namespace Hospital.Api.Endpoints.Exams;

public static class CreateExamEndpoint
{
    public static IEndpointRouteBuilder MapCreateExamEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/exams",
            async (
                CreateExamRequest request,
                CreateExamHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command =
                    new CreateExamCommand(
                        request.PatientId,
                        request.Name,
                        request.RequestedAtUtc);

                var result =
                    await handler.HandleAsync(
                        command,
                        cancellationToken);

                if (result.IsFailure)
                {
                    return Results.BadRequest(
                        new
                        {
                            result.Error.Code,
                            result.Error.Description
                        });
                }

                return Results.Created(
                    $"/exams/{result.Value.Value}",
                    new
                    {
                        Id = result.Value.Value
                    });
            })
            .WithName("CreateExam")
            .WithTags("Exams")
            .WithSummary("Solicita um exame para o paciente")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}

public sealed record CreateExamRequest(
    Guid PatientId,
    string Name,
    DateTimeOffset RequestedAtUtc);