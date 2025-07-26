using Avora.Agent.Core;
using Avora.Agent.Core.Embeddings;
using Avora.Agent.Core.Query;
using Avora.Agent.Core.Tokens;
using FluentAssertions;

namespace Avora.Agent.CoreTests.Query;

public class QueryContextTests
{
    private readonly ITokenizer _tokenizer = new Tokenizer();
    private readonly IEmbeddingModel _embedding = new StaticEmbeddingModel();

    [Fact]
    public void QueryContext_Should_Contain_Query_And_Trace()
    {
        // Arrange
        var input = "abc";
        var query = new Core.Query.Query(input, _tokenizer, _embedding);

        // Act
        var context = new QueryContext(query);

        // Assert
        context.Query.Should().Be(query);
        context.Trace.Should().NotBeNull();
        context.Trace.Count.Should().Be(0);
    }

    [Fact]
    public void AddStep_Should_Record_Trace_Entry()
    {
        // Arrange
        var query = new Core.Query.Query("find", _tokenizer, _embedding);
        var ctx = new QueryContext(query);

        // Act
        ctx.AddStep("Stage1", "tokenized input");

        // Assert
        ctx.Trace.Count.Should().Be(1);
        ctx.Trace.Steps[0].Label.Should().Be("Stage1");
        ctx.Trace.Steps[0].Description.Should().Be("tokenized input");
    }

    [Fact]
    public void ToString_Should_Summarize_Context()
    {
        // Arrange
        var query = new Core.Query.Query("hi", _tokenizer, _embedding);
        var ctx = new QueryContext(query);

        // Act
        var str = ctx.ToString();

        // Assert
        str.Should().Contain("Query");
        str.Should().Contain("TraceSteps=0");
    }
}
