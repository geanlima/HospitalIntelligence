using System.Data.Common;
using Hospital.Salux.Configuration;
using Hospital.Salux.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace Hospital.Salux.Adapters;

public interface ISaluxPatientReader
{
    Task<IReadOnlyList<SaluxPatientRecord>> ReadSinceAsync(
        DateTimeOffset checkpointUtc,
        int batchSize,
        CancellationToken cancellationToken = default);
}

public sealed class SqlSaluxPatientReader : ISaluxPatientReader
{
    private readonly SaluxOptions _options;

    public SqlSaluxPatientReader(IOptions<SaluxOptions> options)
    {
        _options = options.Value;
    }

    public async Task<IReadOnlyList<SaluxPatientRecord>> ReadSinceAsync(
        DateTimeOffset checkpointUtc,
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException(
                "Salux:ConnectionString is required for real Salux sync.");
        }

        if (string.IsNullOrWhiteSpace(_options.PatientQuery))
        {
            throw new InvalidOperationException(
                "Salux:PatientQuery is required.");
        }

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = _options.PatientQuery;
        command.CommandTimeout = 120;

        AddParameter(command, "Checkpoint", checkpointUtc.UtcDateTime);
        AddParameter(command, "BatchSize", batchSize);

        var rows = new List<SaluxPatientRecord>();

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            rows.Add(
                new SaluxPatientRecord(
                    reader.GetString(reader.GetOrdinal("PatientCode")),
                    reader.GetString(reader.GetOrdinal("PatientName")),
                    ReadDateOnly(reader, "BirthDate"),
                    Convert.ToInt32(reader.GetValue(reader.GetOrdinal("GenderCode"))),
                    ReadDateTimeOffset(reader, "UpdatedAtUtc")));
        }

        return rows;
    }

    private DbConnection CreateConnection()
    {
        return _options.Provider.Trim().ToLowerInvariant() switch
        {
            "sqlserver" or "mssql" =>
                new SqlConnection(_options.ConnectionString),
            "oracle" =>
                new OracleConnection(_options.ConnectionString),
            _ => throw new InvalidOperationException(
                $"Unsupported Salux provider '{_options.Provider}'. Use SqlServer or Oracle.")
        };
    }

    private static void AddParameter(
        DbCommand command,
        string name,
        object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = command is OracleCommand
            ? name
            : $"@{name}";
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private static DateOnly ReadDateOnly(DbDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal))
        {
            return DateOnly.MinValue;
        }

        var value = reader.GetValue(ordinal);
        return value switch
        {
            DateOnly dateOnly => dateOnly,
            DateTime dateTime => DateOnly.FromDateTime(dateTime),
            DateTimeOffset dto => DateOnly.FromDateTime(dto.UtcDateTime),
            _ => DateOnly.FromDateTime(Convert.ToDateTime(value))
        };
    }

    private static DateTimeOffset ReadDateTimeOffset(
        DbDataReader reader,
        string column)
    {
        var ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal))
        {
            return DateTimeOffset.UnixEpoch;
        }

        var value = reader.GetValue(ordinal);
        return value switch
        {
            DateTimeOffset dto => dto.ToUniversalTime(),
            DateTime dateTime =>
                new DateTimeOffset(
                    DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)),
            _ => new DateTimeOffset(
                DateTime.SpecifyKind(
                    Convert.ToDateTime(value),
                    DateTimeKind.Utc))
        };
    }
}
