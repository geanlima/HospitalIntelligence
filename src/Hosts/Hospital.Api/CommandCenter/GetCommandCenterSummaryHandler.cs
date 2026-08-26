using Hospital.Dashboard.Application.Dashboard;
using Hospital.Dashboard.Contracts.Dashboard;
using Hospital.ML.Application.Abstractions;

namespace Hospital.Api.CommandCenter;

public sealed record CommandCenterSummaryResponse(
    DashboardSummaryResponse Operational,
    int PredictedDischargesToday,
    int ElevatedDeteriorationCount,
    int HighNoShowRiskCount,
    IReadOnlyList<CommandCenterPatientInsight> TopInsights,
    DateTimeOffset GeneratedAtUtc);

public sealed record CommandCenterPatientInsight(
    Guid PatientId,
    string PatientName,
    string DischargeLabel,
    double DischargeScore,
    string DeteriorationLabel,
    double DeteriorationScore);

public sealed class GetCommandCenterSummaryHandler
{
    private readonly GetDashboardSummaryHandler _dashboard;
    private readonly IMlFeatureSource _featureSource;
    private readonly IMlPredictionService _predictionService;
    private readonly Hospital.Patients.Application.Abstractions.IPatientRepository _patients;

    public GetCommandCenterSummaryHandler(
        GetDashboardSummaryHandler dashboard,
        IMlFeatureSource featureSource,
        IMlPredictionService predictionService,
        Hospital.Patients.Application.Abstractions.IPatientRepository patients)
    {
        _dashboard = dashboard;
        _featureSource = featureSource;
        _predictionService = predictionService;
        _patients = patients;
    }

    public async Task<CommandCenterSummaryResponse> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var operational =
            await _dashboard.HandleAsync(
                new GetDashboardSummaryQuery(),
                cancellationToken);

        var patients =
            await _patients.SearchAsync(null, null, cancellationToken);

        var insights = new List<CommandCenterPatientInsight>();
        var predictedDischarges = 0;
        var elevatedDeterioration = 0;
        var highNoShow = 0;

        foreach (var patient in patients.Take(25))
        {
            var features =
                await _featureSource.GetPatientFeaturesAsync(
                    patient.Id.Value,
                    cancellationToken);

            if (features is null)
            {
                continue;
            }

            var discharge =
                await _predictionService.PredictDischargeAsync(
                    features,
                    cancellationToken);

            var deterioration =
                await _predictionService.PredictDeteriorationAsync(
                    features,
                    cancellationToken);

            var noShow =
                await _predictionService.PredictNoShowAsync(
                    features,
                    cancellationToken);

            if (discharge.Label == "likely-discharge")
            {
                predictedDischarges++;
            }

            if (deterioration.Label == "elevated")
            {
                elevatedDeterioration++;
            }

            if (noShow.Label == "high-risk")
            {
                highNoShow++;
            }

            insights.Add(
                new CommandCenterPatientInsight(
                    patient.Id.Value,
                    patient.Name,
                    discharge.Label,
                    discharge.Score,
                    deterioration.Label,
                    deterioration.Score));
        }

        return new CommandCenterSummaryResponse(
            operational,
            predictedDischarges,
            elevatedDeterioration,
            highNoShow,
            insights
                .OrderByDescending(i => i.DeteriorationScore)
                .Take(10)
                .ToList(),
            DateTimeOffset.UtcNow);
    }
}
