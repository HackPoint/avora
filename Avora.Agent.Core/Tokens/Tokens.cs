namespace Avora.Agent.Core.Tokens;


/// <summary>
/// Represents a token with its value, character span, and position in sequence.
/// </summary>
public readonly record struct Token(
    string Value,
    int StartIndex,
    int End,
    int Length,
    int IndexInSequence) {
    /// <summary>
    /// Simplified constructor that infers Length and IndexInSequence (defaults to -1).
    /// </summary>
    public Token(string value, int start, int end)
        : this(value, start, end, end - start, -1) { }

    /// <summary>
    /// Constructor that also allows setting IndexInSequence manually.
    /// </summary>
    public Token(string value, int start, int end, int indexInSequence)
        : this(value, start, end, end - start, indexInSequence) { }

    public override string ToString()
        => $"[{IndexInSequence}] \"{Value}\" @ ({StartIndex}-{End})";
}