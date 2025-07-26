using Avora.Agent.Core.Embeddings;
using Avora.Agent.Core.Indexing;
using Avora.Agent.Core.Query;
using Avora.Agent.Core.Reasoning;
using Avora.Agent.Core.Storage;
using Avora.Agent.Core.Tokens;
using FluentAssertions;
using Moq;

namespace Avora.Agent.CoreTests.Reasoning;

public class ReasoningEngineTests {
    private readonly Mock<IVectorIndex> _vectorIndex = new();
    private readonly Mock<IDocumentStore> _documentStore = new();

    private static Core.Query.Query CreateTestQuery(string text = "example") {
        var tokens = new List<Token> { new("example", 0, 7) };

        var tokenizer = new Mock<ITokenizer>();
        tokenizer.Setup(t => t.Tokenize(It.IsAny<string>())).Returns(tokens);
        tokenizer.Setup(t => t.RemoveStopWords(It.IsAny<IReadOnlyList<Token>>())).Returns(tokens);

        var model = new Mock<IEmbeddingModel>();
        model.Setup(m => m.Embed(It.IsAny<string>())).Returns([0.1f, 0.2f, 0.3f]);

        return new Core.Query.Query(text, tokenizer.Object, model.Object);
    }

    [Fact]
    public void Index_Should_Upsert_Into_Index_And_Store() {
        // Arrange
        var query = CreateTestQuery();
        var chunk = new DocumentChunk(QueryId.From(Guid.NewGuid()), "data", [0.1f, 0.2f, 0.3f], null, 0);

        var engine = new ReasoningEngine(_vectorIndex.Object, _documentStore.Object);

        // Act
        engine.Index(query, chunk);

        // Assert
        _vectorIndex.Verify(i => i.Upsert(query, chunk.Id), Times.Once);
        _documentStore.Verify(s => s.Upsert(chunk), Times.Once);
    }

    [Fact]
    public void Run_Should_Return_DocumentChunks_From_Store() {
        // Arrange
        var query = CreateTestQuery();
        var id = QueryId.From(Guid.NewGuid());

        _vectorIndex.Setup(i => i.Search(query.Embedding, 5))
            .Returns(new List<(QueryId, float)> { (id, 0.95f) });

        var expectedChunk = new DocumentChunk(id, "test", [0.1f, 0.2f, 0.3f], "source.md", 1);
        _documentStore.Setup(s => s.GetChunks(It.IsAny<IEnumerable<QueryId>>()))
            .Returns(new List<DocumentChunk> { expectedChunk });

        var engine = new ReasoningEngine(_vectorIndex.Object, _documentStore.Object);

        // Act
        var result = engine.Run(query);

        // Assert
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(expectedChunk);
    }

    [Fact]
    public void Delete_Should_Remove_From_Index_And_Store() {
        // Arrange
        var id = QueryId.From(Guid.NewGuid());
        var engine = new ReasoningEngine(_vectorIndex.Object, _documentStore.Object);

        // Act
        engine.Delete(id);

        // Assert
        _vectorIndex.Verify(i => i.Delete(id), Times.Once);
        _documentStore.Verify(s => s.Delete(id), Times.Once);
    }
}