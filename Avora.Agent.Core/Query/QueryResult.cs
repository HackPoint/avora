using System.Collections.ObjectModel;
using Avora.Agent.Core.Trace;

namespace Avora.Agent.Core.Query;

/// <summary>
/// Represents the outcome of a vector-based query,
/// including matched results and trace information.
/// </summary>
public sealed class QueryResult {
    /// <summary>The input query that was executed.</summary>
    public Query Query { get; }

    /// <summary>List of matched results with score and original ID.</summary>
    public IReadOnlyList<ScoredMatch> Matches { get; }

    /// <summary>Trace of reasoning steps recorded during query execution.</summary>
    public ReasoningTrace Trace { get; }

    /// <summary>
    /// Initializes a new query result.
    /// </summary>
    public QueryResult(Query query, IEnumerable<ScoredMatch> matches, ReasoningTrace trace) {
        Query = query ?? throw new ArgumentNullException(nameof(query));
        Matches = new ReadOnlyCollection<ScoredMatch>(matches.ToList());
        Trace = trace ?? new ReasoningTrace();
    }

    public override string ToString()
        => $"QueryResult[{Matches.Count} matches, Top={Matches.FirstOrDefault().Score:F2}]";
}

/// <summary>
/// Represents a single match result with a score.
/// </summary>
public readonly struct ScoredMatch {
    /// <summary>Query ID of the match.</summary>
    public QueryId Id { get; }

    /// <summary>Cosine similarity or distance-based score (0.0–1.0).</summary>
    public float Score { get; }

    public ScoredMatch(QueryId id, float score) {
        Id = id;
        Score = score;
    }

    public override string ToString() => $"{Id} (score: {Score:F3})";
}