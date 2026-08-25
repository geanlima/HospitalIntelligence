using Hospital.Alerts.Application.Alerts.Abstractions;
using Hospital.Alerts.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Alerts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAlertsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "AlertsDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'AlertsDatabase' was not found.");
        }

        services.AddDbContext<AlertsDbContext>(
            options =>
                options.UseNpgsql(
                    connectionString));

        services.AddScoped<
            IPatientAlertRepository,
            PatientAlertRepository>();

        return services;
    }
}