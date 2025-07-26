using Avora.Agent.Core.Embeddings;
using FluentAssertions;

namespace Avora.Agent.CoreTests.Embeddings;

public class EmbeddingVectorTests {
    [Fact]
    public void Constructor_Should_SetValues() {
        // Проверяем, что значения корректно инициализируются
        var vector = new EmbeddingVector([1f, 2f, 3f]);

        vector.Values.Should().Equal(1f, 2f, 3f);
        vector.Count.Should().Be(3);
    }

    [Fact]
    public void CosineSimilarity_Should_Be1_For_IdenticalVectors() {
        var a = new EmbeddingVector([1f, 0f]);
        var b = new EmbeddingVector([1f, 0f]);

        // Один и тот же вектор — косинус равен 1
        a.CosineSimilarity(b).Should().BeApproximately(1f, 0.0001f);
    }

    [Fact]
    public void CosineSimilarity_Should_Be0_For_OrthogonalVectors() {
        var a = new EmbeddingVector([1f, 0f]);
        var b = new EmbeddingVector([0f, 1f]);

        // Перпендикулярные векторы — косинус равен 0
        a.CosineSimilarity(b).Should().BeApproximately(0f, 0.0001f);
    }

    [Fact]
    public void CosineSimilarity_Should_Throw_On_DifferentLengths() {
        var a = new EmbeddingVector([1f, 2f]);
        var b = new EmbeddingVector([1f, 2f, 3f]);

        // Ошибка, если векторы разной длины
        var action = () => a.CosineSimilarity(b);
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Normalized_Should_Produce_UnitLengthVector() {
        var vector = new EmbeddingVector([3f, 4f]);
        var normalized = vector.Normalized();

        // Ожидаем длину 1 (нормированный вектор)
        normalized.Length.Should().BeApproximately(1f, 0.0001f);
    }

    [Fact]
    public void Length_Should_Be_EuclideanNorm() {
        var vector = new EmbeddingVector([3f, 4f]);
        vector.Length.Should().BeApproximately(5f, 0.0001f); // √(9+16) = 5
    }

    [Fact]
    public void ToString_Should_Display_Vector_Info() {
        var vector = new EmbeddingVector([1f, 2f, 3f]);
        vector.ToString().Should().Contain("1");
        vector.ToString().Should().Contain("3");
    }

    [Fact]
    public void ZeroVector_Should_Not_Throw_On_Length() {
        var vector = new EmbeddingVector([0f, 0f, 0f]);
        vector.Length.Should().Be(0f);
    }

    [Fact]
    public void Equality_Should_Compare_Values() {
        var a = new EmbeddingVector([1f, 2f, 3f]);
        var b = new EmbeddingVector([1f, 2f, 3f]);
        var c = new EmbeddingVector([1f, 2f]);

        a.Should().Be(b);
        a.Should().NotBe(c);
    }
}