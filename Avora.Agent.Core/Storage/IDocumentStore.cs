using Avora.Agent.Core.Query;

namespace Avora.Agent.Core.Storage;

/// <summary>
/// Defines a contract for storing and retrieving document chunks.
/// Used in retrieval phase of the RAG pipeline.
/// </summary>
public interface IDocumentStore {
    /// <summary>
    /// Add or update a document chunk in the store.
    /// </summary>
    void Upsert(DocumentChunk chunk);

    /// <summary>
    /// Delete a chunk by ID.
    /// </summary>
    void Delete(QueryId id);

    /// <summary>
    /// Retrieve all chunks matching provided IDs.
    /// </summary>
    IReadOnlyList<DocumentChunk> GetChunks(IEnumerable<QueryId> ids);
}