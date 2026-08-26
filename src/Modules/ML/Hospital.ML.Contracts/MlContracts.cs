namespace Hospital.ML.Contracts;

public sealed record MlPredictionRequest(
    Guid PatientId,
    int AgeYears,
    int LengthOfStayDays,
    int ActiveAlertCount,
    int PendingExamCount,
    double? LatestSpo2,
    double? LatestHeartRate,
    int ActivePrescriptionCount,
    bool HasMedicalNote);

public sealed record MlPredictionDto(
    string ModelName,
    string ModelVersion,
    double Score,
    string Label,
    IReadOnlyDictionary<string, double> Features,
    DateTimeOffset PredictedAtUtc);

public sealed record MlModelInfoDto(
    string Name,
    string Version,
    string Algorithm,
    IReadOnlyDictionary<string, double> Metrics,
    DateTimeOffset TrainedAtUtc,
    bool DriftDetected,
    string DriftNotes);

public sealed record PatientMlInsightsResponse(
    Guid PatientId,
    MlPredictionDto NoShowRisk,
    MlPredictionDto DischargeProbability,
    MlPredictionDto DeteriorationRisk,
    IReadOnlyList<MlModelInfoDto> Models);
