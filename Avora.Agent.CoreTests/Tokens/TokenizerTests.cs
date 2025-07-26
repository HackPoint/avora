using Avora.Agent.Core;
using FluentAssertions;

namespace Avora.Agent.CoreTests.Tokens;

public class TokenizerTests {
    private readonly Tokenizer _tokenizer = new();

    [Fact]
    public void Tokenize_Should_Handle_EmptyInput() {
        var result = _tokenizer.Tokenize(string.Empty);
        result.Should().BeEmpty();
    }

    [Fact]
    public void Tokenize_Should_Lowercase_Input() {
        var result = _tokenizer.Tokenize("HELLO World");
        result.Select(t => t.Value).Should().ContainInOrder("hello", "world");
    }

    [Fact]
    public void Tokenize_Should_Extract_Words_And_Punctuation() {
        var input = "Hello, Avora!";
        var result = _tokenizer.Tokenize(input);

        result.Should().HaveCount(4);
        result.Select(t => t.Value).Should().ContainInOrder("hello", ",", "avora", "!");
        result[0].StartIndex.Should().Be(0);
        result[1].StartIndex.Should().Be(5);
        result[2].StartIndex.Should().Be(7);
        result[3].StartIndex.Should().Be(12);
    }

    [Fact]
    public void Normalize_Should_Return_Joined_Lowercase_Tokens() {
        var input = "This is Avora!";
        var result = _tokenizer.Normalize(input);

        result.Should().Be("this is avora !");
    }

    [Fact]
    public void RemoveStopWords_Should_Remove_StopWord_This() {
        var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize("this is the future");
        var filtered = tokenizer.RemoveStopWords(tokens);

        filtered.Select(t => t.Value).Should().NotContain("this");
        filtered.Select(t => t.Value).Should().Contain("future");
    }

    [Fact]
    public void Token_Should_Have_Expected_Fields() {
        var result = _tokenizer.Tokenize("Avora rocks!");
        var token = result.First();

        token.Value.Should().Be("avora");
        token.StartIndex.Should().Be(0);
        token.Length.Should().Be(5);
        token.IndexInSequence.Should().Be(0);
    }

    [Fact]
    public void Tokenize_Should_Handle_Numbers_And_Symbols() {
        var result = _tokenizer.Tokenize("Version 2.0, price: $10!");

        result.Select(t => t.Value).Should().ContainInOrder(
            "version", "2", ".", "0", ",", "price", ":", "$", "10", "!"
        );
    }
}