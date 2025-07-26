namespace Avora.Agent.Core.Tokens;

/// <summary>
/// Interface for tokenizer implementations that split input into semantic tokens and normalize text.
/// </summary>
public interface ITokenizer {
    /// <summary>
    /// Splits input into tokens.
    /// </summary>
    List<Token> Tokenize(string input);

    /// <summary>
    /// Removes stop words from tokens.
    /// </summary>
    List<Token> RemoveStopWords(IEnumerable<Token> tokens);
}