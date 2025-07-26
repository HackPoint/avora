using Avora.Agent.Core;
using Avora.Agent.Core.Embeddings;
using Avora.Agent.Core.Query;
using Avora.Agent.Core.Trace;
using FluentAssertions;

namespace Avora.Agent.CoreTests.Query;

public class QueryResultTests {
    private readonly Core.Query.Query _query = new("test query", new Tokenizer(), new StaticEmbeddingModel());

    [Fact]
    public void QueryResult_Should_Store_Query_And_Matches() {
        // Arrange
        var trace = new ReasoningTrace();
        trace.Add("stage1", "normalized");

        var matches = new List<ScoredMatch> {
            new(QueryId.From(Guid.NewGuid()), 0.87f),
            new(QueryId.From(Guid.NewGuid()), 0.71f)
        };

        // Act
        var result = new QueryResult(_query, matches, trace);

        // Assert
        result.Query.Should().Be(_query);
        result.Matches.Should().HaveCount(2);
        result.Trace.Should().Be(trace);
    }

    [Fact]
    public void ScoredMatch_Should_Hold_Id_And_Score() {
        // Arrange
        var id = QueryId.From(Guid.NewGuid());

        // Act
        var match = new ScoredMatch(id, 0.91f);

        // Assert
        match.Id.Should().Be(id);
        match.Score.Should().BeApproximately(0.91f, 0.001f);
        match.ToString().Should().Contain("score");
    }

    [Fact]
    public void QueryResult_ToString_Should_Include_Match_Info() {
        // Arrange
        var matches = new List<ScoredMatch> {
            new(QueryId.From(Guid.NewGuid()), 0.92f)
        };

        var result = new QueryResult(_query, matches, new ReasoningTrace());

        // Act
        var str = result.ToString();

        // Assert
        str.Should().Contain("QueryResult");
        str.Should().Contain("matches");
        str.Should().Contain("0.92");
    }
}