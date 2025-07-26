using Avora.Agent.Core.Query;
using FluentAssertions;

namespace Avora.Agent.CoreTests.Query;

public class QueryIdTests {
    [Fact]
    public void New_Should_Generate_Unique_Id() {
        var id1 = QueryId.New();
        var id2 = QueryId.New();

        id1.Should().NotBe(id2);
        id1.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Should_Support_Equality() {
        var guid = Guid.NewGuid();
        var id1 = new QueryId(guid);
        var id2 = new QueryId(guid);

        id1.Should().Be(id2);
        (id1 == id2).Should().BeTrue();
        (id1 != id2).Should().BeFalse();
    }

    [Fact]
    public void ToString_Should_Return_Guid_String() {
        var guid = Guid.NewGuid();
        var id = new QueryId(guid);

        id.ToString().Should().Be(guid.ToString());
    }
}