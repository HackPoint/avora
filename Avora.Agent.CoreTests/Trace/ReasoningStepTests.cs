using Avora.Agent.Core.Trace;
using FluentAssertions;

namespace Avora.Agent.CoreTests.Trace;

public class ReasoningStepTests {
    [Fact]
    public void ReasoningStep_Should_Preserve_Values() {
        var timestamp = DateTime.UtcNow;
        var step = new ReasoningStep("label", "desc", timestamp);

        step.Label.Should().Be("label");
        step.Description.Should().Be("desc");
        step.Timestamp.Should().BeCloseTo(timestamp, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ToString_Should_Include_Label_And_Description() {
        var ts = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var step = new ReasoningStep("Decision", "Matched 5 candidates", ts);

        step.ToString().Should().Contain("Decision");
        step.ToString().Should().Contain("Matched 5 candidates");
        step.ToString().Should().Contain("12:00:00");
    }
}