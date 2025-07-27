using Avora.Agent.Core.Indexing;
using Avora.Agent.Core.Query;
using Avora.Agent.Core.RAG;
using Avora.Agent.Core.Storage;

namespace Avora.Agent.Core.Reasoning;

/// <summary>
/// Responsible for executing reasoning steps using a vector index and document store.
/// </summary>
public sealed class ReasoningEngine(IVectorIndex index, IDocumentStore store) : IRagPipeline {
    public void SetChunks(Query.Query query, IEnumerable<DocumentChunk> chunks) {
        foreach (var chunk in chunks) {
            index.Upsert(query, chunk.Id);
            store.Upsert(chunk);
        }
    }

    /// <summary>
    /// Executes a top-k similarity search over the vector index and returns enriched document chunks.
    /// </summary>
    public IReadOnlyCollection<DocumentChunk> Run(Query.Query query, int k = 5) {
        var matches = index.Search(query.Embedding, k);
        var ids = matches.Select(m => m.Id);
        var chunks = store.GetChunks(ids);
        return chunks;
    }

    /// <summary>
    /// Adds a query and its embedding to the vector index and associates a document chunk in the store.
    /// </summary>
    public void Index(Query.Query query, DocumentChunk chunk) {
        index.Upsert(query, chunk.Id);
        store.Upsert(chunk);
    }

    /// <summary>
    /// Deletes the query and associated chunk from the index and store.
    /// </summary>
    public void Delete(QueryId id) {
        index.Delete(id);
        store.Delete(id);
    }
}