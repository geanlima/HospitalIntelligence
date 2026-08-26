namespace Hospital.Dashboard.Contracts.Dashboard;

public sealed record DashboardActivityResponse(
    Guid Id,
    DateTimeOffset OccurredAtUtc,
    string Type,
    string Title,
    string Description);