using Hospital.ClinicalNotes.Application.ClinicalNotes.Abstractions;
using Hospital.ClinicalNotes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.ClinicalNotes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddClinicalNotesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "ClinicalNotesDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'ClinicalNotesDatabase' was not found.");
        }

        services.AddDbContext<ClinicalNotesDbContext>(
            options =>
                options.UseNpgsql(connectionString));

        services.AddScoped<
            IClinicalNoteRepository,
            ClinicalNoteRepository>();

        return services;
    }
}