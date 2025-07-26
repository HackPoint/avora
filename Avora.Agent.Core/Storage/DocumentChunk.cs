using Avora.Agent.Core.Query;

namespace Avora.Agent.Core.Storage;

/// <summary>
/// Represents a chunk of a document that has been embedded and indexed.
/// Used for retrieval and reasoning steps in RAG.
/// </summary>
public sealed record DocumentChunk(
    QueryId Id,          // Unique ID for the chunk
    string Content,      // Raw content of the document chunk
    float[] Embedding,   // Vector embedding of the content
    string? Source = null, // Optional file or URL source
    int Index = 0        // Index of chunk within its source
);