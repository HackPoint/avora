namespace Avora.Agent.Core.Trace;

/// <summary>
/// Represents a reasoning trace for a query, storing a timeline of logical steps
/// taken by the system to answer a query or reach a conclusion.
/// </summary>
public sealed class ReasoningTrace {
    private readonly List<ReasoningStep> _steps = new();

    /// <summary>
    /// Number of recorded steps in the reasoning trace.
    /// </summary>
    public int Count => _steps.Count;

    /// <summary>
    /// Read-only view of reasoning steps.
    /// </summary>
    public IReadOnlyList<ReasoningStep> Steps => _steps;

    /// <summary>
    /// Adds a labeled step to the trace.
    /// </summary>
    public void Add(string label, string description) {
        _steps.Add(new ReasoningStep(label, description, DateTime.UtcNow));
    }

    /// <summary>
    /// Returns the full reasoning trace as a human-readable list.
    /// </summary>
    public override string ToString()
        => string.Join(Environment.NewLine, _steps.Select(s => s.ToString()));
}