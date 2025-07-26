using System.Net.Http.Json;
using System.Text.Json;

namespace Avora.Agent.Infrastructure.Qdrant;

/// <summary>
/// Handles low-level REST calls to a Qdrant instance.
/// </summary>
public class QdrantClient(HttpClient http, string collectionName) {
    public HttpClient Http => http;
    public string CollectionName => collectionName;

    public async Task EnsureCollectionAsync() {
        var response = await http.PutAsJsonAsync($"/collections/{collectionName}", new {
            vectors = new {
                size = 32,
                distance = "Cosine"
            }
        });
        response.EnsureSuccessStatusCode();
    }

    public async Task UpsertAsync(string id, float[] vector) {
        var body = new {
            points = new[] {
                new {
                    id,
                    vector
                }
            }
        };

        var response = await http.PutAsJsonAsync($"/collections/{collectionName}/points", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<(string Id, float Score)>> SearchAsync(float[] query, int topK) {
        var body = new {
            vector = query,
            top = topK,
            with_payload = true,
            with_vector = false
        };

        var response = await http.PostAsJsonAsync($"/collections/{collectionName}/points/search", body);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<SearchResult>();
        return result?.result.Select(r => (r.id, r.score)).ToList() ?? new();
    }

    public async Task DeleteAsync(string id) {
        var body = new {
            points = new[] { id }
        };

        var response = await http.PostAsJsonAsync($"/collections/{collectionName}/points/delete", body);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<QdrantPoint>> GetPointAsync(IEnumerable<string> ids) {
        var response = await http.PostAsJsonAsync($"/collections/{collectionName}/points", new {
            ids = ids.ToArray()
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetPointsResponse>();
        return result?.result.points ?? new();
    }

    private class SearchResult {
        public List<ResultItem> result { get; set; } = new();

        public class ResultItem {
            public string id { get; set; }
            public float score { get; set; }
        }
    }

    public class QdrantPoint {
        public string id { get; set; } = string.Empty;

        public float[] vector { get; set; } = [];

        public Dictionary<string, JsonElement> payload { get; set; } = new();
    }

    private class QdrantGetResponse {
        public List<QdrantPoint> result { get; set; } = new();
    }
    
    private class GetPointsResponse {
        public PointsContainer result { get; set; } = new();
    }

    private class PointsContainer {
        public List<QdrantPoint> points { get; set; } = new();
    }
}