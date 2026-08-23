using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hospital.Patients.Infrastructure.Persistence;

public sealed class PatientsDbContextFactory
    : IDesignTimeDbContextFactory<PatientsDbContext>
{
    public PatientsDbContext CreateDbContext(
        string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "HOSPITAL_PATIENTS_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Environment variable 'HOSPITAL_PATIENTS_CONNECTION_STRING' was not found.");
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<PatientsDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString);

        return new PatientsDbContext(
            optionsBuilder.Options);
    }
}