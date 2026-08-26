using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pgvector.EntityFrameworkCore;

namespace Hospital.AI.Infrastructure.Persistence;

public sealed class AiDbContextFactory
    : IDesignTimeDbContextFactory<AiDbContext>
{
    public AiDbContext CreateDbContext(
        string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "HOSPITAL_AI_CONNECTION_STRING");

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
            new DbContextOptionsBuilder<AiDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString,
            o => o.UseVector());

        return new AiDbContext(
            optionsBuilder.Options);
    }
}
