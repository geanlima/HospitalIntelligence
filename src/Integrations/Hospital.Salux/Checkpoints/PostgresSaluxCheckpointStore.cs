using Npgsql;

namespace Hospital.Salux.Checkpoints;

public interface ISaluxCheckpointStore
{
    Task<DateTimeOffset> GetAsync(
        string streamKey,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        string streamKey,
        DateTimeOffset checkpointUtc,
        CancellationToken cancellationToken = default);
}

public sealed class PostgresSaluxCheckpointStore : ISaluxCheckpointStore
{
    private readonly string _connectionString;

    public PostgresSaluxCheckpointStore(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task EnsureSchemaAsync(
        CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS salux_sync_checkpoints (
                stream_key varchar(100) PRIMARY KEY,
                checkpoint_utc timestamptz NOT NULL,
                updated_at_utc timestamptz NOT NULL
            );

            CREATE TABLE IF NOT EXISTS salux_idempotency (
                message_id uuid PRIMARY KEY,
                processed_at_utc timestamptz NOT NULL
            );
            """;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<DateTimeOffset> GetAsync(
        string streamKey,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT checkpoint_utc
            FROM salux_sync_checkpoints
            WHERE stream_key = @streamKey;
            """;
        command.Parameters.AddWithValue("streamKey", streamKey);

        var result = await command.ExecuteScalarAsync(cancellationToken);

        if (result is null || result is DBNull)
        {
            return DateTimeOffset.UnixEpoch;
        }

        return (DateTimeOffset)result;
    }

    public async Task SaveAsync(
        string streamKey,
        DateTimeOffset checkpointUtc,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO salux_sync_checkpoints (stream_key, checkpoint_utc, updated_at_utc)
            VALUES (@streamKey, @checkpoint, @updatedAt)
            ON CONFLICT (stream_key) DO UPDATE
            SET checkpoint_utc = EXCLUDED.checkpoint_utc,
                updated_at_utc = EXCLUDED.updated_at_utc;
            """;
        command.Parameters.AddWithValue("streamKey", streamKey);
        command.Parameters.AddWithValue("checkpoint", checkpointUtc.UtcDateTime);
        command.Parameters.AddWithValue("updatedAt", DateTime.UtcNow);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
