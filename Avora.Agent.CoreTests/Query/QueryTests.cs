using Avora.Agent.Core;
using Avora.Agent.Core.Embeddings;
using Avora.Agent.Core.Tokens;
using FluentAssertions;

namespace Avora.Agent.CoreTests.Query;

public class QueryTests {
    private readonly ITokenizer _tokenizer = new Tokenizer();
    private readonly IEmbeddingModel _embedding = new StaticEmbeddingModel();

    [Fact]
    public void Query_Should_Tokenize_And_Embed_Text() {
        // Arrange
        var input = "AI is the future of intelligence, right?";

        // Act
        var query = new Core.Query.Query(input, _tokenizer, _embedding);

        // Assert
        query.OriginalText.Should().Be(input);
        query.Tokens.Should().NotBeNull().And.HaveCountGreaterThan(0);
        query.NormalizedText.Should().NotContain("the"); // стоп-слово
        query.Embedding.Should().NotBeNull().And.HaveCount(32);
    }

    [Fact]
    public void NormalizedText_Should_Remove_StopWords() {
        // Arrange
        var input = "This is the real test for ai agent";

        // Act
        var query = new Core.Query.Query(input, _tokenizer, _embedding);

        // Assert
        query.NormalizedText.Should().NotContain("this");
        query.NormalizedText.Should().NotContain("is");
        query.NormalizedText.Should().Contain("ai");
        query.NormalizedText.Should().Contain("agent");
    }

    [Fact]
    public void Embedding_Should_Be_Deterministic() {
        // Arrange
        var input = "reasoning structure";

        // Act
        var q1 = new Core.Query.Query(input, _tokenizer, _embedding);
        var q2 = new Core.Query.Query(input, _tokenizer, _embedding);

        // Assert
        q1.Embedding.Should().Equal(q2.Embedding);
    }

    [Fact]
    public void ToString_Should_Return_NormalizedText() {
        var input = "The Tokenizer should be tested";
        var query = new Core.Query.Query(input, _tokenizer, _embedding);

        query.ToString().Should().Be(query.NormalizedText);
    }
}