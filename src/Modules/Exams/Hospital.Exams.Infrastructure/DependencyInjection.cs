using Hospital.Exams.Application.Exams.Abstractions;
using Hospital.Exams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Exams.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection
        AddExamsInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "ExamsDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'ExamsDatabase' was not found.");
        }

        services.AddDbContext<ExamsDbContext>(
            options =>
                options.UseNpgsql(
                    connectionString));

        services.AddScoped<
            IExamRepository,
            ExamRepository>();

        return services;
    }
}