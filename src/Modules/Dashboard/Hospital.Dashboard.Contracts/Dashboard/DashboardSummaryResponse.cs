namespace Hospital.Dashboard.Contracts.Dashboard;

public sealed record DashboardSummaryResponse(
    int TotalPatients,
    int ActiveAdmissions,
    int PendingExams,
    int CriticalAlerts,
    int TotalBeds,
    int OccupiedBeds,
    int AvailableBeds,
    decimal OccupancyPercentage,
    IReadOnlyCollection<DashboardActivityResponse> RecentActivities);