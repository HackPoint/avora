
using System.Text.Json.Serialization;

namespace Avora.Agent.Core.Embeddings;

public readonly record struct EmbeddingResult(float[] Vector, string SourceText);
public class EmbedResult
{
    [JsonPropertyName("embeddings")]
    public List<float[]> Embeddings { get; set; } = new();
}