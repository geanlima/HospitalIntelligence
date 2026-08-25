using Hospital.VitalSigns.Application.VitalSigns.CreateVitalSign;

namespace Hospital.Api.Endpoints.VitalSigns;

public static class CreateVitalSignEndpoint
{
    public static IEndpointRouteBuilder MapCreateVitalSignEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/vital-signs",
            async (
                CreateVitalSignRequest request,
                CreateVitalSignHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateVitalSignCommand(
                    request.PatientId,
                    request.MeasuredAtUtc,
                    request.Temperature,
                    request.HeartRate,
                    request.RespiratoryRate,
                    request.SystolicBloodPressure,
                    request.DiastolicBloodPressure,
                    request.OxygenSaturation);

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
                    $"/vital-signs/{result.Value.Value}",
                    new
                    {
                        Id = result.Value.Value
                    });
            })
            .WithName("CreateVitalSign")
            .WithTags("Vital Signs")
            .WithSummary("Registra os sinais vitais de um paciente")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}

public sealed record CreateVitalSignRequest(
    Guid PatientId,
    DateTimeOffset MeasuredAtUtc,
    decimal? Temperature,
    int? HeartRate,
    int? RespiratoryRate,
    int? SystolicBloodPressure,
    int? DiastolicBloodPressure,
    decimal? OxygenSaturation);