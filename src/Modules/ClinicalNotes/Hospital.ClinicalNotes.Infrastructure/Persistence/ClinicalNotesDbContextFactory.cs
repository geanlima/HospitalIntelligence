using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hospital.ClinicalNotes.Infrastructure.Persistence;

public sealed class ClinicalNotesDbContextFactory
    : IDesignTimeDbContextFactory<ClinicalNotesDbContext>
{
    public ClinicalNotesDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "HOSPITAL_CLINICALNOTES_CONNECTION_STRING");

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
            new DbContextOptionsBuilder<ClinicalNotesDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new ClinicalNotesDbContext(
            optionsBuilder.Options);
    }
}