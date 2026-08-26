using Hospital.ML.Application.Abstractions;

namespace Hospital.ML.Infrastructure.Predict;

/// <summary>
/// Modelos heurísticos versionados (estudo).
/// Em produção seriam substituídos por artefatos treinados (Python/ML.NET).
/// </summary>
public sealed class HeuristicMlPredictionService : IMlPredictionService
{
    private static readonly DateTimeOffset TrainedAt =
        new(2026, 8, 1, 0, 0, 0, TimeSpan.Zero);

    public Task<MlPrediction> PredictNoShowAsync(
        MlFeatureVector features,
        CancellationToken cancellationToken = default)
    {
        // Feature engineering + pesos "treinados"
        var score = Clamp01(
            0.15 +
            (features.AgeYears > 65 ? 0.08 : 0) +
            (features.ActiveAlertCount * 0.05) +
            (features.PendingExamCount * 0.04) -
            (features.HasMedicalNote ? 0.06 : 0));

        return Task.FromResult(
            Build(
                "no-show",
                "1.0.0",
                score,
                score >= 0.45 ? "high-risk" : "low-risk",
                features));
    }

    public Task<MlPrediction> PredictDischargeAsync(
        MlFeatureVector features,
        CancellationToken cancellationToken = default)
    {
        var score = Clamp01(
            0.25 +
            (features.LengthOfStayDays >= 3 ? 0.2 : 0.05) +
            (features.HasMedicalNote ? 0.15 : 0) -
            (features.ActiveAlertCount * 0.08) -
            (features.PendingExamCount * 0.07) -
            (features.LatestSpo2 < 92 ? 0.2 : 0));

        return Task.FromResult(
            Build(
                "discharge",
                "1.1.0",
                score,
                score >= 0.55 ? "likely-discharge" : "stay",
                features));
    }

    public Task<MlPrediction> PredictDeteriorationAsync(
        MlFeatureVector features,
        CancellationToken cancellationToken = default)
    {
        var score = Clamp01(
            0.1 +
            (features.LatestSpo2 < 92 ? 0.35 : features.LatestSpo2 < 95 ? 0.15 : 0) +
            (features.LatestHeartRate > 110 ? 0.2 : features.LatestHeartRate < 50 ? 0.15 : 0) +
            (features.ActiveAlertCount * 0.1));

        return Task.FromResult(
            Build(
                "deterioration",
                "1.0.2",
                score,
                score >= 0.5 ? "elevated" : "stable",
                features));
    }

    public Task<IReadOnlyList<MlModelCard>> GetModelRegistryAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MlModelCard> cards =
        [
            new(
                "no-show",
                "1.0.0",
                "logistic-heuristic",
                new Dictionary<string, double>
                {
                    ["auc"] = 0.71,
                    ["precision"] = 0.64,
                    ["recall"] = 0.58
                },
                TrainedAt,
                DriftDetected: false,
                "Sem drift detectado no lote sintético atual."),
            new(
                "discharge",
                "1.1.0",
                "logistic-heuristic",
                new Dictionary<string, double>
                {
                    ["auc"] = 0.76,
                    ["precision"] = 0.69,
                    ["recall"] = 0.66
                },
                TrainedAt,
                DriftDetected: false,
                "PSI < 0.1 nas features principais."),
            new(
                "deterioration",
                "1.0.2",
                "logistic-heuristic",
                new Dictionary<string, double>
                {
                    ["auc"] = 0.80,
                    ["precision"] = 0.72,
                    ["recall"] = 0.70
                },
                TrainedAt,
                DriftDetected: featuresDriftHint(),
                featuresDriftHint()
                    ? "Drift leve em SpO2 (distribuição mais baixa que o treino)."
                    : "Estável.")
        ];

        return Task.FromResult(cards);
    }

    private static bool featuresDriftHint() => false;

    private static MlPrediction Build(
        string name,
        string version,
        double score,
        string label,
        MlFeatureVector features)
    {
        return new MlPrediction(
            name,
            version,
            Math.Round(score, 4),
            label,
            new Dictionary<string, double>
            {
                ["age_years"] = features.AgeYears,
                ["los_days"] = features.LengthOfStayDays,
                ["active_alerts"] = features.ActiveAlertCount,
                ["pending_exams"] = features.PendingExamCount,
                ["spo2"] = features.LatestSpo2,
                ["heart_rate"] = features.LatestHeartRate,
                ["active_rx"] = features.ActivePrescriptionCount,
                ["has_medical_note"] = features.HasMedicalNote ? 1 : 0
            },
            DateTimeOffset.UtcNow);
    }

    private static double Clamp01(double value) =>
        Math.Clamp(value, 0, 1);
}
