using Avora.Agent.Core.Indexing;
using Avora.Agent.Core.Query;

namespace Avora.Agent.Infrastructure.Qdrant;

/// <summary>
/// Provides a production-ready vector index backed by Qdrant.
/// </summary>
public class QdrantVectorIndex : IVectorIndex {
    private readonly QdrantClient _client;

    public QdrantVectorIndex(QdrantClient client) {
        _client = client;
    }
    public async Task InitializeAsync() {
        await _client.EnsureCollectionAsync();
    }

    public void Upsert(Query query, QueryId id) {
        _client.UpsertAsync(id.Value.ToString(), query.Embedding).Wait();
    }

    public List<(QueryId Id, float Score)> Search(float[] embedding, int k = 5) {
        var result = _client.SearchAsync(embedding, k).Result;
        return result.Select(r => (new QueryId(Guid.Parse(r.Id)), r.Score)).ToList();
    }

    public void Delete(QueryId id) {
        _client.DeleteAsync(id.Value.ToString()).Wait();
    }
}
