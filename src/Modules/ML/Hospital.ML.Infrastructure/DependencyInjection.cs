using Hospital.ML.Application.Abstractions;
using Hospital.ML.Application.Predict;
using Hospital.ML.Infrastructure.Predict;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.ML.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMlInfrastructure(
        this IServiceCollection services)
    {
        services.AddSingleton<IMlPredictionService, HeuristicMlPredictionService>();
        services.AddScoped<GetPatientMlInsightsHandler>();
        return services;
    }
}
