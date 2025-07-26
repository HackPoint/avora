using Avora.Agent.Core.Embeddings;
using Avora.Agent.Core.Indexing;
using Avora.Agent.Core.Query;
using Avora.Agent.Core.Storage;
using Avora.Agent.Core.Tokens;

namespace Avora.Agent.Core.RAG;

/// <summary>
/// Production RAG pipeline implementation using embedding model, vector index, and store.
/// </summary>
public class RagPipeline : IRagPipeline {
    private readonly IEmbeddingModel _embeddingModel;
    private readonly ITokenizer _tokenizer;
    private readonly IVectorIndex _vectorIndex;
    private readonly IDocumentStore _documentStore;

    public RagPipeline(
        IEmbeddingModel embeddingModel,
        ITokenizer tokenizer,
        IVectorIndex vectorIndex,
        IDocumentStore documentStore) {
        _embeddingModel = embeddingModel;
        _tokenizer = tokenizer;
        _vectorIndex = vectorIndex;
        _documentStore = documentStore;
    }

    public void SetChunks(Query.Query query, IEnumerable<DocumentChunk> chunks) {
        foreach (var chunk in chunks) {
            Index(query, chunk);
        }
    }

    public IReadOnlyCollection<DocumentChunk> Run(Query.Query query, int k = 5) {
        var results = _vectorIndex.Search(query.Embedding, k);
        var ids = results.Select(r => r.Id);
        return _documentStore.GetChunks(ids);
    }

    public void Index(Query.Query query, DocumentChunk chunk) {
        _vectorIndex.Upsert(query, chunk.Id);
        _documentStore.Upsert(chunk);
    }

    public void Delete(QueryId id) {
        _vectorIndex.Delete(id);
        _documentStore.Delete(id);
    }

    public Query.Query CreateQuery(string input) {
        return new Query.Query(input, _tokenizer, _embeddingModel);
    }
}