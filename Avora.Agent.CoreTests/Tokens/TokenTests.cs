using Avora.Agent.Core;
using Avora.Agent.Core.Tokens;
using FluentAssertions;

namespace Avora.Agent.CoreTests.Tokens;

public class TokenTests {
    [Fact]
    public void Token_Equality_Should_Work() {
        var a = new Token("data", 0, 4, 0);
        var b = new Token("data", 0, 4, 0);

        a.Should().Be(b);
        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Token_With_Different_Index_Should_Not_Be_Equal() {
        var a = new Token("data", 0, 4, 0);
        var b = new Token("data", 0, 4, 1);

        a.Should().NotBe(b);
    }

    [Fact]
    public void Token_Struct_Should_Have_Proper_Fields()
    {
        var token = new Token("ai", 2, 4, 3);

        token.Value.Should().Be("ai");
        token.StartIndex.Should().Be(2);
        token.End.Should().Be(4);
        token.Length.Should().Be(2); // computed internally
        token.IndexInSequence.Should().Be(3);
    }

    [Fact]
    public void Token_Should_Be_Immutable() {
        var token = new Token("avora", 0, 5, 0);
        var modified = token with { Value = "agent" };

        token.Value.Should().Be("avora");
        modified.Value.Should().Be("agent");
    }
}