using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hospital.Admissions.Infrastructure.Persistence;

public sealed class AdmissionsDbContextFactory
    : IDesignTimeDbContextFactory<AdmissionsDbContext>
{
    public AdmissionsDbContext CreateDbContext(
        string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "HOSPITAL_ADMISSIONS_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Environment variable 'HOSPITAL_ADMISSIONS_CONNECTION_STRING' was not found.");
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<AdmissionsDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString);

        return new AdmissionsDbContext(
            optionsBuilder.Options);
    }
}