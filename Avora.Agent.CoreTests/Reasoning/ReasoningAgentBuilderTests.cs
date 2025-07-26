using Avora.Agent.Core.Agent;
using Avora.Agent.Core.Builders;
using Avora.Agent.Core.Embeddings;
using Avora.Agent.Core.Indexing;
using Avora.Agent.Core.Storage;
using Avora.Agent.Core.Tokens;
using FluentAssertions;
using Moq;

namespace Avora.Agent.CoreTests.Reasoning;

public class ReasoningAgentBuilderTests {
    
    [Fact]
    public void Build_Should_Construct_Agent_With_All_Required_Dependencies() {
        // Arrange
        var tokenizer = new Mock<ITokenizer>().Object;
        var embeddingModel = new Mock<IEmbeddingModel>().Object;
        var textGenerator = new Mock<ITextGenerator>().Object;
        var vectorIndex = new Mock<IVectorIndex>().Object;
        var documentStore = new Mock<IDocumentStore>().Object;

        // Act
        var agent = new ReasoningAgentBuilder()
            .WithTokenizer(tokenizer)
            .WithEmbeddingModel(embeddingModel)
            .WithTextGenerator(textGenerator)
            .WithVectorIndex(vectorIndex)
            .WithDocumentStore(documentStore)
            .Build();

        // Assert
        agent.Should().NotBeNull();
        agent.Should().BeAssignableTo<IAgent>();
    }
    
    [Theory]
    [InlineData(false, true, true, true, true, "Tokenizer is required")]
    [InlineData(true, false, true, true, true, "EmbeddingModel is required")]
    [InlineData(true, true, false, true, true, "TextGenerator is required")]
    [InlineData(true, true, true, false, true, "VectorIndex is required")]
    [InlineData(true, true, true, true, false, "DocumentStore is required")]
    public void Build_Should_Throw_If_Dependency_Is_Missing(
        bool hasTokenizer, bool hasEmbedding, bool hasTextGen,
        bool hasVector, bool hasStore, string expectedMessage
    ) {
        var builder = new ReasoningAgentBuilder();

        if (hasTokenizer) builder.WithTokenizer(new Mock<ITokenizer>().Object);
        if (hasEmbedding) builder.WithEmbeddingModel(new Mock<IEmbeddingModel>().Object);
        if (hasTextGen) builder.WithTextGenerator(new Mock<ITextGenerator>().Object);
        if (hasVector) builder.WithVectorIndex(new Mock<IVectorIndex>().Object);
        if (hasStore) builder.WithDocumentStore(new Mock<IDocumentStore>().Object);

        var act = () => builder.Build();

        act.Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);
    }

}