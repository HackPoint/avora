namespace Avora.Agent.Core.Embedding;

/// <summary>
/// Represents a floating-point vector used for embedding texts or queries
/// in a high-dimensional semantic space.
/// </summary>
public sealed class EmbeddingVector {
    /// <summary>
    /// Underlying values of the embedding vector.
    /// </summary>
    public IReadOnlyList<float> Values { get; }

    /// <summary>
    /// Number of dimensions in the vector.
    /// </summary>
    public int Count => Values.Count;

    /// <summary>
    /// Access individual component by index.
    /// </summary>
    public float this[int index] => Values[index];

    /// <summary>
    /// Creates a new embedding vector from float values.
    /// </summary>
    public EmbeddingVector(IEnumerable<float> values) {
        if (values is null)
            throw new ArgumentNullException(nameof(values));

        Values = values.ToArray();
    }

    /// <summary>
    /// Calculates the Euclidean norm (length) of the vector.
    /// This is used to normalize or compare vectors by magnitude.
    /// </summary>
    public float Length => (float)Math.Sqrt(Values.Sum(v => v * v));

    /// <summary>
    /// Returns a new normalized vector with length = 1.
    /// This is used before cosine similarity to ensure magnitude doesn’t affect similarity.
    /// </summary>
    public EmbeddingVector Normalized() {
        var length = Length;
        if (length == 0)
            return new EmbeddingVector(Values.Select(_ => 0f)); // prevent divide-by-zero

        return new EmbeddingVector(Values.Select(v => v / length));
    }

    /// <summary>
    /// Calculates cosine similarity between this vector and another.
    /// Returns a value between -1 and 1:
    /// • 1 = same direction
    /// • 0 = orthogonal
    /// • -1 = opposite direction
    /// </summary>
    public float CosineSimilarity(EmbeddingVector other) {
        if (other is null)
            throw new ArgumentNullException(nameof(other));
        if (other.Count != Count)
            throw new InvalidOperationException("Vectors must be of the same length.");

        float dot = 0f, mag1 = 0f, mag2 = 0f;

        for (int i = 0; i < Count; i++) {
            float a = Values[i];
            float b = other.Values[i];
            dot += a * b;
            mag1 += a * a;
            mag2 += b * b;
        }

        if (mag1 == 0 || mag2 == 0)
            return 0f;

        return dot / (float)Math.Sqrt(mag1 * mag2);
    }

    /// <summary>
    /// String representation for debugging purposes.
    /// </summary>
    public override string ToString()
        => $"EmbeddingVector[{string.Join(", ", Values)}]";

    public override bool Equals(object? obj)
        => obj is EmbeddingVector other && Values.SequenceEqual(other.Values);

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            foreach (var v in Values)
                hash = hash * 31 + v.GetHashCode();
            return hash;
        }
    }
}