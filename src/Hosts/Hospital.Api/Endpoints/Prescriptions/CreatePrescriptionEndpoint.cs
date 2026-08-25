using Hospital.Prescriptions.Application.CreatePrescription;

namespace Hospital.Api.Endpoints.Prescriptions;

public static class CreatePrescriptionEndpoint
{
    public static IEndpointRouteBuilder MapCreatePrescriptionEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/prescriptions",
            async (
                CreatePrescriptionRequest request,
                CreatePrescriptionHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreatePrescriptionCommand(
                    request.PatientId,
                    request.Description,
                    request.PrescribedAtUtc);

                var result = await handler.HandleAsync(
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
                    $"/prescriptions/{result.Value.Value}",
                    new
                    {
                        Id = result.Value.Value
                    });
            })
            .WithName("CreatePrescription")
            .WithTags("Prescriptions")
            .WithSummary("Cria uma prescrição para o paciente")
            .WithDescription(
                "Cria uma nova prescrição vinculada a um paciente.")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}

public sealed record CreatePrescriptionRequest(
    Guid PatientId,
    string Description,
    DateTimeOffset PrescribedAtUtc);