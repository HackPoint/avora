using Avora.Agent.Core.Embeddings;
using FluentAssertions;

namespace Avora.Agent.CoreTests.Embeddings;

public class StaticEmbeddingModelTests {
    private readonly StaticEmbeddingModel _model = new();

    [Fact]
    public void Embed_Should_Return_Vector_Of_Fixed_Length() {
        // Arrange
        var input = "avora agent";

        // Act
        var vector = _model.Embed(input);

        // Assert
        vector.Should().NotBeNull();
        vector.Length.Should().Be(32);
        vector.All(v => v is >= 0f and <= 1f).Should().BeTrue();
    }

    [Fact]
    public void Embed_Should_Be_Deterministic() {
        // Arrange
        var input = "the same input";

        // Act
        var v1 = _model.Embed(input);
        var v2 = _model.Embed(input);

        // Assert
        v1.Should().Equal(v2); // determinism test
    }

    [Fact]
    public void EmbedMany_Should_Return_Same_Results_As_Single() {
        // Arrange
        var inputs = new[] { "one", "two", "three" };

        // Act
        var many = _model.EmbedMany(inputs);
        var separately = inputs.Select(i => _model.Embed(i)).ToList();

        // Assert
        many.Count.Should().Be(separately.Count);

        for (int i = 0; i < many.Count; i++) {
            many[i].Should().BeEquivalentTo(separately[i], options => options.WithStrictOrdering());
        }
    }
}