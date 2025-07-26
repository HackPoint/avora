using Avora.Agent.Core.Agent;
using Avora.Agent.Core.Embeddings;
using Avora.Agent.Core.Indexing;
using Avora.Agent.Core.RAG;
using Avora.Agent.Core.Storage;
using Avora.Agent.Core.Tokens;

namespace Avora.Agent.Core.Builders;

/// <summary>
/// Fluent builder for constructing a fully configured ReasoningAgent.
/// </summary>
public class ReasoningAgentBuilder {
    private ITokenizer? _tokenizer;
    private IEmbeddingModel? _embeddingModel;
    private ITextGenerator? _textGenerator;
    private IVectorIndex? _vectorIndex;
    private IDocumentStore? _documentStore;

    public ReasoningAgentBuilder WithTokenizer(ITokenizer tokenizer) {
        _tokenizer = tokenizer;
        return this;
    }

    public ReasoningAgentBuilder WithEmbeddingModel(IEmbeddingModel embeddingModel) {
        _embeddingModel = embeddingModel;
        return this;
    }

    public ReasoningAgentBuilder WithVectorIndex(IVectorIndex index) {
        _vectorIndex = index;
        return this;
    }

    public ReasoningAgentBuilder WithDocumentStore(IDocumentStore store) {
        _documentStore = store;
        return this;
    }

    public ReasoningAgentBuilder WithTextGenerator(ITextGenerator generator) {
        _textGenerator = generator;
        return this;
    }

    public IAgent Build() {
        if (_tokenizer == null) throw new InvalidOperationException("Tokenizer is required");
        if (_embeddingModel == null) throw new InvalidOperationException("EmbeddingModel is required");
        if (_textGenerator == null) throw new InvalidOperationException("TextGenerator is required");
        if (_vectorIndex == null) throw new InvalidOperationException("VectorIndex is required");
        if (_documentStore == null) throw new InvalidOperationException("DocumentStore is required");

        var ragPipeline = new RagPipeline(_embeddingModel, _tokenizer, _vectorIndex, _documentStore);
        return new ReasoningAgent(ragPipeline, _textGenerator);
    }
}