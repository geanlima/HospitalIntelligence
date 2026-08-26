using Hospital.Admissions.Application.Admissions.Abstractions;
using Hospital.Alerts.Application.Alerts.Abstractions;
using Hospital.Dashboard.Contracts.Dashboard;
using Hospital.Exams.Application.Exams.Abstractions;
using Hospital.Patients.Application.Abstractions;
using Hospital.Timeline.Application.Timeline.Abstractions;
using Microsoft.Extensions.Options;

namespace Hospital.Dashboard.Application.Dashboard;

public sealed class GetDashboardSummaryHandler
{
    private readonly IPatientRepository _patientRepository;
    private readonly IAdmissionRepository _admissionRepository;
    private readonly IExamRepository _examRepository;
    private readonly IPatientAlertRepository _patientAlertRepository;
    private readonly ITimelineRepository _timelineRepository;
    private readonly HospitalCapacityOptions _capacityOptions;

    public GetDashboardSummaryHandler(
        IPatientRepository patientRepository,
        IAdmissionRepository admissionRepository,
        IExamRepository examRepository,
        IPatientAlertRepository patientAlertRepository,
        ITimelineRepository timelineRepository,
        IOptions<HospitalCapacityOptions> capacityOptions)
    {
        _patientRepository = patientRepository;
        _admissionRepository = admissionRepository;
        _examRepository = examRepository;
        _patientAlertRepository = patientAlertRepository;
        _timelineRepository = timelineRepository;
        _capacityOptions = capacityOptions.Value;
    }

    public async Task<DashboardSummaryResponse> HandleAsync(
        GetDashboardSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        var totalPatients =
            await _patientRepository.CountAsync(cancellationToken);

        var activeAdmissions =
            await _admissionRepository.CountActiveAsync(cancellationToken);

        var pendingExams =
            await _examRepository.CountPendingAsync(cancellationToken);

        var criticalAlerts =
            await _patientAlertRepository.CountCriticalAsync(cancellationToken);

        var recentTimelineItems =
            await _timelineRepository.GetRecentAsync(
                5,
                cancellationToken);

        var totalBeds = _capacityOptions.TotalBeds;
        var occupiedBeds = activeAdmissions;

        var availableBeds = Math.Max(
            totalBeds - occupiedBeds,
            0);

        var occupancyPercentage =
            totalBeds > 0
                ? Math.Round(
                    (decimal)occupiedBeds / totalBeds * 100,
                    2)
                : 0;

        var recentActivities = recentTimelineItems
            .Select(x => new DashboardActivityResponse(
                x.Id.Value,
                x.OccurredAtUtc,
                x.Type,
                x.Title,
                x.Description))
            .ToList()
            .AsReadOnly();

        return new DashboardSummaryResponse(
            TotalPatients: totalPatients,
            ActiveAdmissions: activeAdmissions,
            PendingExams: pendingExams,
            CriticalAlerts: criticalAlerts,
            TotalBeds: totalBeds,
            OccupiedBeds: occupiedBeds,
            AvailableBeds: availableBeds,
            OccupancyPercentage: occupancyPercentage,
            RecentActivities: recentActivities);
    }
}