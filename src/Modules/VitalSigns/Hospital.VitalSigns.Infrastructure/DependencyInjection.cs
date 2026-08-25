using Hospital.VitalSigns.Application.VitalSigns.Abstractions;
using Hospital.VitalSigns.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.VitalSigns.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddVitalSignsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "VitalSignsDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'VitalSignsDatabase' was not found.");
        }

        services.AddDbContext<VitalSignsDbContext>(
            options =>
                options.UseNpgsql(
                    connectionString));

        services.AddScoped<
            IVitalSignRepository,
            VitalSignRepository>();

        return services;
    }
}