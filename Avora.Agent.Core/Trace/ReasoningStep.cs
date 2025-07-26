namespace Avora.Agent.Core.Trace;

/// <summary>
/// Represents an individual reasoning step in a query trace.
/// </summary>
public sealed record ReasoningStep(string Label, string Description, DateTime Timestamp) {
    public override string ToString()
        => $"[{Timestamp:HH:mm:ss}] {Label}: {Description}";
}