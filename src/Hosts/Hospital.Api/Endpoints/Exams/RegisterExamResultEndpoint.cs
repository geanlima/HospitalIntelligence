using Hospital.Api.Common;
using Hospital.Exams.Application.Exams.RegisterExamResult;
using Hospital.Exams.Domain.Exams;

namespace Hospital.Api.Endpoints.Exams;

public static class RegisterExamResultEndpoint
{
    public static IEndpointRouteBuilder MapRegisterExamResultEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/exams/{id:guid}/result",
            async (
                Guid id,
                RegisterExamResultRequest request,
                RegisterExamResultHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command =
                    new RegisterExamResultCommand(
                        new ExamId(id),
                        request.Result,
                        request.ResultedAtUtc);

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
            .WithName("RegisterExamResult")
            .WithTags("Exams")
            .WithSummary("Registra o resultado de um exame")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}

public sealed record RegisterExamResultRequest(
    string Result,
    DateTimeOffset ResultedAtUtc);