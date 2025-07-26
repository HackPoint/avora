using System.Net.Http.Json;
using System.Text.Json;
using Avora.Agent.Core.Query;
using Avora.Agent.Core.Storage;

namespace Avora.Agent.Infrastructure.Qdrant;

/// <summary>
/// Stores document chunks in Qdrant as payloads associated with their vector IDs.
/// Used for retrieving full documents after vector search.
/// </summary>
public sealed class QdrantDocumentStore(QdrantClient client) : IDocumentStore {
    public void Upsert(DocumentChunk chunk) {
        var payload = new Dictionary<string, object> {
            ["content"] = chunk.Content,
            ["source"] = chunk.Source ?? string.Empty,
            ["index"] = chunk.Index
        };

        // Embedding with payload
        var body = new {
            points = new[] {
                new {
                    id = chunk.Id.ToString(),
                    vector = chunk.Embedding,
                    payload
                }
            }
        };

        var response = client.Http.PutAsJsonAsync($"/collections/{client.CollectionName}/points", body).Result;
        response.EnsureSuccessStatusCode();
    }

    public void Delete(QueryId id) {
        client.DeleteAsync(id.ToString()).GetAwaiter().GetResult();
    }

    public IReadOnlyList<DocumentChunk> GetChunks(IEnumerable<QueryId> ids) {
        var stringIds = ids.Select(id => id.ToString()).ToList();

        // Get points with vectors and payload's
        var points = client.GetPointAsync(stringIds).Result;

        var result = new List<DocumentChunk>();

        foreach (var point in points) {
            if (point.vector is not { Length: > 0 } vector || point.payload is null)
                continue;

            // Read payload (from JsonElement)
            var payload = point.payload;

            payload.TryGetValue("content", out var contentEl);
            payload.TryGetValue("source", out var sourceEl);
            payload.TryGetValue("index", out var indexEl);

            var content = contentEl.GetString() ?? string.Empty;
            var source = sourceEl.ValueKind == JsonValueKind.String ? sourceEl.GetString() : null;
            var index = indexEl.ValueKind == JsonValueKind.Number ? indexEl.GetInt32() : 0;

            var queryId = QueryId.From(point.id); // string → QueryId

            result.Add(new DocumentChunk(queryId, content, vector, source, index));
        }

        return result;
    }
}