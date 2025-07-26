namespace Avora.Agent.Core.Tokens;

public static class StopWordSet {
    public static readonly HashSet<string> Default = new(StringComparer.OrdinalIgnoreCase) {
        "this", "the", "and", "or", "but", "to", "a", "an", "of", "in", "on", "is", "are", "with", "for", "as", "at", "by"
    };
}