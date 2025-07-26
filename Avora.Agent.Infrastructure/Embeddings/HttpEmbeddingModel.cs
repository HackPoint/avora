using System.Net.Http.Json;
using Avora.Agent.Core.Embeddings;

namespace Avora.Agent.Infrastructure.Embeddings;

public class HttpEmbeddingModel(HttpClient http) : IEmbeddingModel {
    public float[] Embed(string text) {
        var response = http.PostAsJsonAsync("/embed", new { text }).Result;
        response.EnsureSuccessStatusCode();
        var result = response.Content.ReadFromJsonAsync<float[]>().Result;
        return result!;
    }

    public IReadOnlyList<float[]> EmbedMany(IEnumerable<string> inputs) {
        var request = new { texts = inputs };
        var response = http.PostAsJsonAsync("/embed-many", request).Result;
        response.EnsureSuccessStatusCode();
        var result = response.Content.ReadFromJsonAsync<List<float[]>>().Result;
        return result!;
    }
}