using Hospital.Integration.Idempotency;
using Npgsql;

namespace Hospital.Salux.Idempotency;

public sealed class PostgresSaluxIdempotencyStore : IIdempotencyStore
{
    private readonly string _connectionString;

    public PostgresSaluxIdempotencyStore(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> HasBeenProcessedAsync(
        Guid messageId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT 1
            FROM salux_idempotency
            WHERE message_id = @messageId
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("messageId", messageId);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null && result is not DBNull;
    }

    public async Task MarkAsProcessedAsync(
        Guid messageId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO salux_idempotency (message_id, processed_at_utc)
            VALUES (@messageId, @processedAt)
            ON CONFLICT (message_id) DO NOTHING;
            """;
        command.Parameters.AddWithValue("messageId", messageId);
        command.Parameters.AddWithValue("processedAt", DateTime.UtcNow);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
