namespace Avora.Agent.Core.Embeddings;

/// <summary>
/// Interface for embedding models that convert text into numerical vector representations.
/// </summary>
public interface IEmbeddingModel {
    /// <summary>
    /// Embeds a single input string into a dense vector.
    /// </summary>
    float[] Embed(string input);

    /// <summary>
    /// Embeds multiple input strings (batch).
    /// </summary>
    IReadOnlyList<float[]> EmbedMany(IEnumerable<string> inputs);
}