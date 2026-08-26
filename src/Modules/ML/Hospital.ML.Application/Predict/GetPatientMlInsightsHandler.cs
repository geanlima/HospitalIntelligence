using Hospital.ML.Application.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.ML.Application.Predict;

public sealed record GetPatientMlInsightsQuery(Guid PatientId);

public sealed record PatientMlInsightsResult(
    Guid PatientId,
    MlPrediction NoShowRisk,
    MlPrediction DischargeProbability,
    MlPrediction DeteriorationRisk,
    IReadOnlyList<MlModelCard> Models);

public sealed class GetPatientMlInsightsHandler
{
    private readonly IMlFeatureSource _featureSource;
    private readonly IMlPredictionService _predictionService;

    public GetPatientMlInsightsHandler(
        IMlFeatureSource featureSource,
        IMlPredictionService predictionService)
    {
        _featureSource = featureSource;
        _predictionService = predictionService;
    }

    public async Task<Result<PatientMlInsightsResult>> HandleAsync(
        GetPatientMlInsightsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.PatientId == Guid.Empty)
        {
            return Result<PatientMlInsightsResult>.Failure(
                new Error(
                    "ML.PatientIdRequired",
                    "PatientId é obrigatório."));
        }

        var features =
            await _featureSource.GetPatientFeaturesAsync(
                query.PatientId,
                cancellationToken);

        if (features is null)
        {
            return Result<PatientMlInsightsResult>.Failure(
                new Error(
                    "ML.PatientNotFound",
                    "Paciente não encontrado para feature engineering."));
        }

        var noShow =
            await _predictionService.PredictNoShowAsync(features, cancellationToken);

        var discharge =
            await _predictionService.PredictDischargeAsync(features, cancellationToken);

        var deterioration =
            await _predictionService.PredictDeteriorationAsync(features, cancellationToken);

        var models =
            await _predictionService.GetModelRegistryAsync(cancellationToken);

        return Result<PatientMlInsightsResult>.Success(
            new PatientMlInsightsResult(
                query.PatientId,
                noShow,
                discharge,
                deterioration,
                models));
    }
}
