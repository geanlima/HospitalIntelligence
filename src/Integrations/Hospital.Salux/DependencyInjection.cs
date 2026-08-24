using Hospital.Salux.Handlers;
using Hospital.Salux.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Salux;

public static class DependencyInjection
{
    public static IServiceCollection AddSaluxIntegration(
        this IServiceCollection services)
    {
        services.AddSingleton<SaluxPatientMapper>();

        services.AddScoped<SaluxPatientMessageHandler>();

        return services;
    }
}