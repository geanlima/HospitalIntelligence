using Hospital.Admissions.Application.Admissions.CreateAdmission;

namespace Hospital.Api.Endpoints.Admissions;

public static class CreateAdmissionEndpoint
{
    public static IEndpointRouteBuilder MapCreateAdmissionEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/admissions",
            async (
                CreateAdmissionRequest request,
                CreateAdmissionHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command =
                    new CreateAdmissionCommand(
                        request.PatientId,
                        request.AdmissionDate,
                        request.Unit,
                        request.Bed);

                var result =
                    await handler.HandleAsync(
                        command,
                        cancellationToken);

                return Results.Created(
                    $"/admissions/{result.Value.Value}",
                    new
                    {
                        Id = result.Value.Value
                    });
            })
            .WithName("CreateAdmission")
            .WithTags("Admissions")
            .WithSummary("Cria uma internação")
            .Produces(StatusCodes.Status201Created);

        return app;
    }
}

public sealed record CreateAdmissionRequest(
    Guid PatientId,
    DateTimeOffset AdmissionDate,
    string? Unit,
    string? Bed);