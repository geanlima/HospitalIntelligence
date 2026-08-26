namespace Hospital.ML.Application.Abstractions;

public sealed record MlFeatureVector(
    Guid PatientId,
    int AgeYears,
    int LengthOfStayDays,
    int ActiveAlertCount,
    int PendingExamCount,
    double LatestSpo2,
    double LatestHeartRate,
    int ActivePrescriptionCount,
    bool HasMedicalNote);

public sealed record MlPrediction(
    string ModelName,
    string ModelVersion,
    double Score,
    string Label,
    IReadOnlyDictionary<string, double> Features,
    DateTimeOffset PredictedAtUtc);

public sealed record MlModelCard(
    string Name,
    string Version,
    string Algorithm,
    IReadOnlyDictionary<string, double> Metrics,
    DateTimeOffset TrainedAtUtc,
    bool DriftDetected,
    string DriftNotes);

public interface IMlPredictionService
{
    Task<MlPrediction> PredictNoShowAsync(
        MlFeatureVector features,
        CancellationToken cancellationToken = default);

    Task<MlPrediction> PredictDischargeAsync(
        MlFeatureVector features,
        CancellationToken cancellationToken = default);

    Task<MlPrediction> PredictDeteriorationAsync(
        MlFeatureVector features,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MlModelCard>> GetModelRegistryAsync(
        CancellationToken cancellationToken = default);
}

public interface IMlFeatureSource
{
    Task<MlFeatureVector?> GetPatientFeaturesAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}
