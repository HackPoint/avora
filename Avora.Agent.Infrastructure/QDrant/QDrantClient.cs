using System.Net.Http.Json;
using System.Text.Json;

namespace Avora.Agent.Infrastructure.Qdrant;

/// <summary>
/// Handles low-level REST calls to a Qdrant instance.
/// </summary>
public class QdrantClient(HttpClient http, string collectionName) {
    public string CollectionName { get; } = collectionName;

    public async Task EnsureCollectionAsync(int vectorSize = 384) {
        var body = new {
            vectors = new {
                size = vectorSize,
                distance = "Cosine"
            }
        };

        var response = await http.PutAsJsonAsync($"/collections/{CollectionName}", body);
        await EnsureSuccess(response, "EnsureCollectionAsync");
    }

    public async Task UpsertAsync(string id, float[] vector, object? payload = null) {
        var body = new {
            points = new[] {
                new {
                    id,
                    vector,
                    payload
                }
            }
        };

        var response = await http.PutAsJsonAsync($"/collections/{CollectionName}/points", body);
        await EnsureSuccess(response, "UpsertAsync");
    }

    public async Task<List<(string Id, float Score)>> SearchAsync(float[] query, int topK = 5) {
        var body = new {
            vector = query,
            top = topK,
            with_payload = true,
            with_vector = false
        };

        var response = await http.PostAsJsonAsync($"/collections/{CollectionName}/points/search", body);
        await EnsureSuccess(response, "SearchAsync");

        var result = await response.Content.ReadFromJsonAsync<SearchResult>();
        return result?.result.Select(r => (r.id, r.score)).ToList() ?? new();
    }

    public async Task DeleteAsync(string id) {
        var body = new {
            points = new[] { id }
        };

        var response = await http.PostAsJsonAsync($"/collections/{CollectionName}/points/delete", body);
        await EnsureSuccess(response, "DeleteAsync");
    }

    public async Task<List<QdrantPoint>> GetPointAsync(IEnumerable<string> ids) {
        var body = new {
            ids = ids.ToArray(),
            with_payload = true,
            with_vector = true
        };

        var response = await http.PostAsJsonAsync($"/collections/{CollectionName}/points", body);
        await EnsureSuccess(response, "GetPointAsync");

        var result = await response.Content.ReadFromJsonAsync<GetPointsResponse>();
        return result?.result ?? new();
    }

    private static async Task EnsureSuccess(HttpResponseMessage response, string operation) {
        if (!response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Qdrant error in {operation}: {(int)response.StatusCode} {response.ReasonPhrase}\n{content}");
        }
    }

    // Response DTOs
    private class SearchResult {
        public List<ResultItem> result { get; set; } = new();

        public class ResultItem {
            public string id { get; set; } = string.Empty;
            public float score { get; set; }
        }
    }

    public class QdrantPoint {
        public string id { get; set; } = string.Empty;
        public float[] vector { get; set; } = [];
        public Dictionary<string, JsonElement> payload { get; set; } = new();
    }

    private class GetPointsResponse {
        public List<QdrantPoint> result { get; set; } = new();
        public string status { get; set; }
        public float time { get; set; }
    }

    private class PointsContainer {
        public List<QdrantPoint> points { get; set; } = new();
    }
}