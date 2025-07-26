using Avora.Agent.Core.Embeddings;
using Avora.Agent.Core.Tokens;

namespace Avora.Agent.Core.Query;

/// <summary>
/// Represents a preprocessed search/query input for semantic similarity, reasoning, and ranking.
/// </summary>
public sealed class Query {
    public QueryId Id { get; }
    public string OriginalText { get; }
    public string NormalizedText { get; }
    public IReadOnlyList<Token> Tokens { get; }
    public float[] Embedding { get; }

    public Query(string originalText, ITokenizer tokenizer, IEmbeddingModel embeddingModel) {
        Id = QueryId.New();
        OriginalText = originalText;
        Tokens = tokenizer.Tokenize(originalText);
        var filtered = tokenizer.RemoveStopWords(Tokens);
        NormalizedText = string.Join(" ", filtered.Select(t => t.Value));
        Embedding = embeddingModel.Embed(NormalizedText);
    }

    public override string ToString() => NormalizedText;
}

public interface IQueryFactory {
    Query Create(string originalText);
}

public sealed class QueryFactory(ITokenizer tokenizer, IEmbeddingModel embedding) : IQueryFactory {
    public Query Create(string text) => new(text, tokenizer, embedding);
}