using Hospital.Admissions.Application.Admissions.Abstractions;
using Hospital.Admissions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Admissions.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAdmissionsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "AdmissionsDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'AdmissionsDatabase' was not found.");

        services.AddDbContext<AdmissionsDbContext>(
            options =>
                options.UseNpgsql(connectionString));

        services.AddScoped<
            IAdmissionRepository,
            AdmissionRepository>();

        return services;
    }
}