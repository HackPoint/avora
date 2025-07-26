using System.Text.RegularExpressions;

namespace Avora.Agent.Core;

public class Tokenizer {
    private static readonly Regex TokenPattern = new(@"[\p{L}\p{N}\p{M}]+|[\p{P}\p{S}]", RegexOptions.Compiled);

    public List<Token> Tokenize(string input) {
        var tokens = new List<Token>();
        if (string.IsNullOrWhiteSpace(input)) return tokens;

        var lowered = input.ToLowerInvariant();
        var matches = TokenPattern.Matches(lowered);

        for (int i = 0; i < matches.Count; i++) {
            var match = matches[i];
            tokens.Add(new Token(
                Value: match.Value,
                StartIndex: match.Index,
                Length: match.Length,
                IndexInSequence: i
            ));
        }

        return tokens;
    }

    public string Normalize(string input) {
        var tokens = Tokenize(input);
        return string.Join(" ", tokens.Select(t => t.Value));
    }

    public List<Token> RemoveStopWords(IEnumerable<Token> tokens) {
        var stopWords = StopWordSet.Default;
        return tokens.Where(t => !stopWords.Contains(t.Value)).ToList();
    }
}