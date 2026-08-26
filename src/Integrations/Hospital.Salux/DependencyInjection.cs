using Hospital.Integration.Abstractions;
using Hospital.Integration.Idempotency;
using Hospital.Salux.Adapters;
using Hospital.Salux.Checkpoints;
using Hospital.Salux.Configuration;
using Hospital.Salux.Handlers;
using Hospital.Salux.Idempotency;
using Hospital.Salux.Mappers;
using Hospital.Salux.Sync;
using Hospital.Salux.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Salux;

public static class DependencyInjection
{
    public static IServiceCollection AddSaluxIntegration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SaluxOptions>(
            configuration.GetSection(SaluxOptions.SectionName));

        services.PostConfigure<SaluxOptions>(options =>
        {
            if (string.IsNullOrWhiteSpace(options.PatientQueryFile))
            {
                return;
            }

            var path = options.PatientQueryFile.Trim();
            if (File.Exists(path))
            {
                options.PatientQuery = File.ReadAllText(path);
                return;
            }

            if (options.Enabled)
            {
                throw new FileNotFoundException(
                    $"Salux PatientQueryFile not found: {path}",
                    path);
            }
        });

        var options =
            configuration
                .GetSection(SaluxOptions.SectionName)
                .Get<SaluxOptions>() ?? new SaluxOptions();

        var checkpointConnection =
            FirstNonEmpty(
                options.CheckpointConnectionString,
                configuration.GetConnectionString("PatientsDatabase"));

        if (string.IsNullOrWhiteSpace(checkpointConnection))
        {
            throw new InvalidOperationException(
                "Salux checkpoint store requires CheckpointConnectionString or PatientsDatabase.");
        }

        services.AddSingleton(
            new PostgresSaluxCheckpointStore(checkpointConnection));

        services.AddSingleton<ISaluxCheckpointStore>(sp =>
            sp.GetRequiredService<PostgresSaluxCheckpointStore>());

        services.AddSingleton<IIdempotencyStore>(
            _ => new PostgresSaluxIdempotencyStore(checkpointConnection));

        services.AddSingleton<SaluxPatientMapper>();
        services.AddSingleton<ISaluxPatientReader, SqlSaluxPatientReader>();
        services.AddSingleton<SaluxPatientAdapter>();

        services.AddSingleton<IExternalSystemAdapter>(sp =>
            sp.GetRequiredService<SaluxPatientAdapter>());

        services.AddScoped<SaluxPatientMessageHandler>();
        services.AddScoped<SaluxPatientSyncService>();

        services.AddHostedService<SaluxPatientSyncWorker>();

        return services;
    }

    public static async Task EnsureSaluxSchemaAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        var store = services.GetRequiredService<PostgresSaluxCheckpointStore>();
        await store.EnsureSchemaAsync(cancellationToken);
    }

    private static string? FirstNonEmpty(params string?[] values) =>
        values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
}
