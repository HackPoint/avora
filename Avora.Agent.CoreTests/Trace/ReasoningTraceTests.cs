using Avora.Agent.Core.Trace;
using FluentAssertions;

namespace Avora.Agent.CoreTests.Trace;

public class ReasoningTraceTests {
    [Fact]
    public void Add_Should_Add_Step() {
        var trace = new ReasoningTrace();

        trace.Add("step1", "description1");

        trace.Count.Should().Be(1);
        trace.Steps[0].Label.Should().Be("step1");
        trace.Steps[0].Description.Should().Be("description1");
    }

    [Fact]
    public void ToString_Should_Format_All_Steps() {
        var trace = new ReasoningTrace();
        trace.Add("stepA", "started");
        trace.Add("stepB", "completed");

        var str = trace.ToString();

        str.Should().Contain("stepA");
        str.Should().Contain("stepB");
        str.Should().Contain("started");
        str.Should().Contain("completed");
    }
}