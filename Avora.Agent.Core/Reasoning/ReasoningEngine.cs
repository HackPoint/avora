using Avora.Agent.Core.Indexing;
using Avora.Agent.Core.Query;
using Avora.Agent.Core.RAG;
using Avora.Agent.Core.Storage;

namespace Avora.Agent.Core.Reasoning;

/// <summary>
/// Responsible for executing reasoning steps using a vector index and document store.
/// </summary>
public sealed class ReasoningEngine : IRagPipeline {
    private readonly IVectorIndex _index;
    private readonly IDocumentStore _store;

    public ReasoningEngine(IVectorIndex index, IDocumentStore store) {
        _index = index;
        _store = store;
    }

    public void SetChunks(Query.Query query, IEnumerable<DocumentChunk> chunks) {
        foreach (var chunk in chunks) {
            _index.Upsert(query, chunk.Id);
            _store.Upsert(chunk);
        }
    }

    /// <summary>
    /// Executes a top-k similarity search over the vector index and returns enriched document chunks.
    /// </summary>
    public IReadOnlyCollection<DocumentChunk> Run(Query.Query query, int k = 5) {
        var matches = _index.Search(query.Embedding, k);
        var ids = matches.Select(m => m.Id);
        var chunks = _store.GetChunks(ids);
        return chunks;
    }

    /// <summary>
    /// Adds a query and its embedding to the vector index and associates a document chunk in the store.
    /// </summary>
    public void Index(Query.Query query, DocumentChunk chunk) {
        _index.Upsert(query, chunk.Id);
        _store.Upsert(chunk);
    }

    /// <summary>
    /// Deletes the query and associated chunk from the index and store.
    /// </summary>
    public void Delete(QueryId id) {
        _index.Delete(id);
        _store.Delete(id);
    }
}