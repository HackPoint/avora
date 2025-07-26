namespace Avora.Agent.Core;

public static class StopWordSet {
    public static readonly HashSet<string> Default = new(StringComparer.OrdinalIgnoreCase) {
        "the", "and", "or", "but", "to", "a", "an", "of", "in", "on", "is", "are", "with", "for", "as", "at", "by"
    };
}