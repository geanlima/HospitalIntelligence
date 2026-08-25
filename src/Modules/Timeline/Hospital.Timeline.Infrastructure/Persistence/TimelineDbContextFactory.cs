using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hospital.Timeline.Infrastructure.Persistence;

public sealed class TimelineDbContextFactory
    : IDesignTimeDbContextFactory<TimelineDbContext>
{
    public TimelineDbContext CreateDbContext(
        string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "HOSPITAL_TIMELINE_CONNECTION_STRING");

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
            new DbContextOptionsBuilder<TimelineDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString);

        return new TimelineDbContext(
            optionsBuilder.Options);
    }
}