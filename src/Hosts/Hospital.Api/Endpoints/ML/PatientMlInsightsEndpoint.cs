using Hospital.ML.Application.Abstractions;
using Hospital.ML.Application.Predict;
using Hospital.ML.Contracts;
using Hospital.Api.Common;

namespace Hospital.Api.Endpoints.ML;

public static class PatientMlInsightsEndpoint
{
    public static IEndpointRouteBuilder MapPatientMlInsightsEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/ml/patients/{patientId:guid}/insights",
                async (
                    Guid patientId,
                    GetPatientMlInsightsHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await handler.HandleAsync(
                            new GetPatientMlInsightsQuery(patientId),
                            cancellationToken);

                    if (result.IsFailure)
                    {
                        return result.ToProblem();
                    }

                    var value = result.Value;

                    return Results.Ok(
                        new PatientMlInsightsResponse(
                            value.PatientId,
                            ToDto(value.NoShowRisk),
                            ToDto(value.DischargeProbability),
                            ToDto(value.DeteriorationRisk),
                            value.Models.Select(ToModelDto).ToList()));
                })
            .WithTags("ML")
            .WithName("GetPatientMlInsights")
            .WithSummary("Previsões ML do paciente (no-show, alta, deterioração)")
            .Produces<PatientMlInsightsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(
                "/ml/models",
                async (
                    IMlPredictionService predictionService,
                    CancellationToken cancellationToken) =>
                {
                    var models =
                        await predictionService.GetModelRegistryAsync(
                            cancellationToken);

                    return Results.Ok(models.Select(ToModelDto).ToList());
                })
            .WithTags("ML")
            .WithName("GetMlModels")
            .WithSummary("Registry de modelos (versão, métricas, drift)")
            .Produces<IReadOnlyList<MlModelInfoDto>>(StatusCodes.Status200OK);

        return app;
    }

    private static MlPredictionDto ToDto(MlPrediction prediction) =>
        new(
            prediction.ModelName,
            prediction.ModelVersion,
            prediction.Score,
            prediction.Label,
            prediction.Features,
            prediction.PredictedAtUtc);

    private static MlModelInfoDto ToModelDto(MlModelCard model) =>
        new(
            model.Name,
            model.Version,
            model.Algorithm,
            model.Metrics,
            model.TrainedAtUtc,
            model.DriftDetected,
            model.DriftNotes);
}
