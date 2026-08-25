using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hospital.VitalSigns.Infrastructure.Persistence;

public sealed class VitalSignsDbContextFactory
    : IDesignTimeDbContextFactory<VitalSignsDbContext>
{
    public VitalSignsDbContext CreateDbContext(
        string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "HOSPITAL_VITALSIGNS_CONNECTION_STRING");

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
            new DbContextOptionsBuilder<VitalSignsDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString);

        return new VitalSignsDbContext(
            optionsBuilder.Options);
    }
}