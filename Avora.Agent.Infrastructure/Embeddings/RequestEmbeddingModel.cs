using System.Text.Json.Serialization;

namespace Avora.Agent.Infrastructure.Embeddings;

public class EmbedRequest {
    public List<string> Texts { get; set; } = new();
}
