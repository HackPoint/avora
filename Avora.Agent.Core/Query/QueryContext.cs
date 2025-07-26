using Avora.Agent.Core.Trace;


namespace Avora.Agent.Core.Query;

/// <summary>
/// Wraps the full reasoning context for a given query,
/// including trace steps, metadata, and timing information.
/// </summary>
public sealed class QueryContext {
    public Query Query { get; }
    public ReasoningTrace Trace { get; }

    public QueryContext(Query query) {
        Query = query ?? throw new ArgumentNullException(nameof(query));
        Trace = new ReasoningTrace();
    }

    public void AddStep(string label, string description) {
        Trace.Add(label, description);
    }

    public override string ToString()
        => $"QueryContext[{Query}, TraceSteps={Trace.Count}]";
}