using System.Security.Cryptography;
using System.Text;

namespace Avora.Agent.Core.Embeddings;

/// <summary>
/// Simple embedding generator using SHA256 hash mapped to float values (for deterministic demo purposes).
/// </summary>
public sealed class StaticEmbeddingModel : IEmbeddingModel {
    private const int VectorSize = 64;

    public float[] Embed(string input) {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return hash
            .Take(VectorSize)
            .Select(b => (float)b / 255f)
            .ToArray();
    }

    public IReadOnlyList<float[]> EmbedMany(IEnumerable<string> inputs)
        => inputs.Select(Embed).ToList();
}