using Hospital.Prescriptions.Application.Abstractions;
using Hospital.Prescriptions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Prescriptions.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPrescriptionsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "PrescriptionsDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'PrescriptionsDatabase' was not found.");
        }

        services.AddDbContext<PrescriptionsDbContext>(
            options =>
                options.UseNpgsql(
                    connectionString));

        services.AddScoped<
            IPrescriptionRepository,
            PrescriptionRepository>();

        return services;
    }
}