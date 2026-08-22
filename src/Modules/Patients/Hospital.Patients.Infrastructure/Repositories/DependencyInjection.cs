using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Infrastructure.Persistence;
using Hospital.Patients.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Patients.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPatientsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("PatientsDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'PatientsDatabase' was not found.");

        services.AddDbContext<PatientsDbContext>(
            options =>
                options.UseNpgsql(connectionString));

        services.AddScoped<
            IPatientRepository,
            PatientRepository>();

        return services;
    }
}