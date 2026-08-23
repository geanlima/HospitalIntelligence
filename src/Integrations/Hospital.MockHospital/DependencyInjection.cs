using Hospital.Integration.Abstractions;
using Hospital.MockHospital.Adapters;
using Hospital.MockHospital.Handlers;
using Hospital.MockHospital.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.MockHospital;

public static class DependencyInjection
{
    public static IServiceCollection AddMockHospital(
        this IServiceCollection services)
    {
        services.AddSingleton<MockHospitalPatientMapper>();

        services.AddSingleton<MockHospitalAdapter>();

        services.AddSingleton<IExternalSystemAdapter>(
            serviceProvider =>
                serviceProvider.GetRequiredService<
                    MockHospitalAdapter>());

        services.AddScoped<MockHospitalPatientMessageHandler>();

        return services;
    }
}