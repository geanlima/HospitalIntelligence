using System.Security.Cryptography;
using System.Text;
using Hospital.AI.Application.Abstractions;

namespace Hospital.AI.Infrastructure.Embeddings;

/// <summary>
/// Embedding determinístico para estudo (não é um modelo real).
/// Serve para entender a porta IEmbeddingService antes de integrar um provider real.
/// </summary>
public sealed class DeterministicEmbeddingService : IEmbeddingService
{
    public const int Dimensions = 32;

    public Task<EmbeddingVector> EmbedAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var normalized = text.Trim().ToLowerInvariant();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        var values = new float[Dimensions];

        for (var i = 0; i < Dimensions; i++)
        {
            values[i] = hash[i % hash.Length] / 255f;
        }

        var magnitude = MathF.Sqrt(values.Sum(v => v * v));

        if (magnitude > 0)
        {
            for (var i = 0; i < values.Length; i++)
            {
                values[i] /= magnitude;
            }
        }

        return Task.FromResult(new EmbeddingVector(values));
    }
}
