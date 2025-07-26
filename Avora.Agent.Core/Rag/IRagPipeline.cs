using Avora.Agent.Core.Query;
using Avora.Agent.Core.Storage;

namespace Avora.Agent.Core.RAG;

/// <summary>
/// Represents the full RAG pipeline: retrieval and indexing of vectorized queries.
/// </summary>
public interface IRagPipeline {
    /// <summary>
    /// Bulk indexes multiple document chunks for a given query into both the vector index and document store.
    /// Useful for initial dataset ingestion or batch updates.
    /// </summary>
    void SetChunks(Query.Query query, IEnumerable<DocumentChunk> chunks);

    /// <summary>
    /// Executes a search using the vector index and returns relevant document chunks.
    /// </summary>
    IReadOnlyCollection<DocumentChunk> Run(Query.Query query, int k = 5);

    /// <summary>
    /// Adds a query and corresponding document chunk to the index and store.
    /// </summary>
    void Index(Query.Query query, DocumentChunk chunk);

    /// <summary>
    /// Deletes a query and associated document chunk by id.
    /// </summary>
    void Delete(QueryId id);
}
