using Hospital.Timeline.Application.Timeline.Abstractions;
using Hospital.Timeline.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Timeline.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTimelineInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "TimelineDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'TimelineDatabase' was not found.");
        }

        services.AddDbContext<TimelineDbContext>(
            options =>
                options.UseNpgsql(
                    connectionString));

        services.AddScoped<
            ITimelineRepository,
            TimelineRepository>();

        return services;
    }
}