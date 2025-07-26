namespace Avora.Agent.Core;

public readonly record struct Token(
    string Value,
    int StartIndex,
    int Length,
    int IndexInSequence
);