namespace Avora.Agent.Core.Query;


/// <summary>
/// Represents a configuration for executing a vector-based query,
/// including top-K results, score threshold, and semantic filtering options.
/// </summary>
public sealed class QueryExecutionPlan {
    public int TopK { get; }
    public float MinScoreThreshold { get; }
    public bool EnableSemanticFiltering { get; }

    public static readonly QueryExecutionPlan Default = new(topK: 5, minScoreThreshold: 0.0f, enableSemanticFiltering: false);

    public QueryExecutionPlan(int topK, float minScoreThreshold = 0.0f, bool enableSemanticFiltering = false) {
        if (topK <= 0)
            throw new ArgumentOutOfRangeException(nameof(topK), "TopK must be positive");

        if (minScoreThreshold < 0.0f || minScoreThreshold > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(minScoreThreshold), "Score threshold must be between 0.0 and 1.0");

        TopK = topK;
        MinScoreThreshold = minScoreThreshold;
        EnableSemanticFiltering = enableSemanticFiltering;
    }

    public override string ToString()
        => $"Plan[TopK={TopK}, Threshold={MinScoreThreshold}, SemanticFilter={EnableSemanticFiltering}]";
}
