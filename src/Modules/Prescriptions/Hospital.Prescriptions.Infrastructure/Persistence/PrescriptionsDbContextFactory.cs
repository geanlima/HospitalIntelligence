using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hospital.Prescriptions.Infrastructure.Persistence;

public sealed class PrescriptionsDbContextFactory
    : IDesignTimeDbContextFactory<PrescriptionsDbContext>
{
    public PrescriptionsDbContext CreateDbContext(
        string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "HOSPITAL_PRESCRIPTIONS_CONNECTION_STRING");

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
            new DbContextOptionsBuilder<PrescriptionsDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString);

        return new PrescriptionsDbContext(
            optionsBuilder.Options);
    }
}