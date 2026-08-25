using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hospital.Exams.Infrastructure.Persistence;

public sealed class ExamsDbContextFactory
    : IDesignTimeDbContextFactory<ExamsDbContext>
{
    public ExamsDbContext CreateDbContext(
        string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "HOSPITAL_EXAMS_CONNECTION_STRING");

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
            new DbContextOptionsBuilder<ExamsDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString);

        return new ExamsDbContext(
            optionsBuilder.Options);
    }
}