using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hospital.Alerts.Infrastructure.Persistence;

public sealed class AlertsDbContextFactory
    : IDesignTimeDbContextFactory<AlertsDbContext>
{
    public AlertsDbContext CreateDbContext(
        string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "HOSPITAL_ALERTS_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString =
                "Host=localhost;" +
                "Port=5432;" +
                "Database=hospital_intelligence;" +
                "Username=postgres;" +
                "Password=postgres";
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<AlertsDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString);

        return new AlertsDbContext(
            optionsBuilder.Options);
    }
}