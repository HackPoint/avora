using Avora.Agent.Core.Agent;
using Avora.Agent.Core.Embeddings;
using Avora.Agent.Core.Query;
using Avora.Agent.Core.RAG;
using Avora.Agent.Core.Storage;
using Avora.Agent.Core.Tokens;
using FluentAssertions;
using Moq;

namespace Avora.Agent.CoreTests.Agent;

public class ReasoningAgentTests {
    
    [Fact]
    public void Execute_Should_Return_Generated_Text_Based_On_Context() {
        // Arrange
        var chunk = new DocumentChunk(QueryId.New(), "DSA means Data Structures and Algorithms.", new float[3],
            "doc.md");
        var query = new Core.Query.Query("What is DSA?", new MockTokenizer(), new MockEmbeddingModel());

        var rag = new Mock<IRagPipeline>();
        rag.Setup(r => r.Run(It.IsAny<Core.Query.Query>(), 5))
            .Returns(new List<DocumentChunk> { chunk });

        var textGen = new Mock<ITextGenerator>();
        textGen.Setup(g => g.Generate(
            It.IsAny<string>(), It.IsAny<string>()
        )).Returns(new AgentResponse {
            Answer = "DSA stands for Data Structures and Algorithms."
        });

        var agent = new ReasoningAgent(rag.Object, textGen.Object);

        // Act
        var output = agent.Execute(query);

        // Assert
        output.AnswerText.Should().Be("DSA stands for Data Structures and Algorithms.");
    }

    private class MockTokenizer : ITokenizer {
        public List<Token> Tokenize(string input) => [new Token("DSA", 0, 3)];
        public List<Token> RemoveStopWords(IEnumerable<Token> tokens) => tokens.ToList();
    }

    private class MockEmbeddingModel : IEmbeddingModel {
        public float[] Embed(string input) => [0.1f, 0.2f, 0.3f];

        public IReadOnlyList<float[]> EmbedMany(IEnumerable<string> inputs) {
            throw new NotImplementedException();
        }
    }
}