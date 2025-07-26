using Avora.Agent.Core.Query;
using FluentAssertions;

namespace Avora.Agent.CoreTests.Query;

public class QueryExecutionPlanTests {
    [Fact]
    public void Should_Create_Valid_Default_Plan() {
        var plan = QueryExecutionPlan.Default;

        plan.TopK.Should().Be(5);
        plan.MinScoreThreshold.Should().Be(0.0f);
        plan.EnableSemanticFiltering.Should().BeFalse();
    }

    [Fact]
    public void Should_Create_Custom_Valid_Plan() {
        var plan = new QueryExecutionPlan(10, 0.4f, true);

        plan.TopK.Should().Be(10);
        plan.MinScoreThreshold.Should().Be(0.4f);
        plan.EnableSemanticFiltering.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_On_Invalid_TopK(int topK) {
        var act = () => new QueryExecutionPlan(topK);

        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*TopK*");
    }

    [Theory]
    [InlineData(-0.1f)]
    [InlineData(1.5f)]
    public void Should_Throw_On_Invalid_ScoreThreshold(float threshold) {
        var act = () => new QueryExecutionPlan(5, threshold);

        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*Score threshold*");
    }

    [Fact]
    public void ToString_Should_Return_Summary() {
        var plan = new QueryExecutionPlan(3, 0.5f, true);

        var str = plan.ToString();
        str.Should().Contain("TopK=3");
        str.Should().Contain("Threshold=0.5");
        str.Should().Contain("SemanticFilter=True");
    }
}