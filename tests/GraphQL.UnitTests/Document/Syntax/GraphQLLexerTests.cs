using FluentAssertions;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.GraphQL.Document.Syntax;

public class GraphQLLexerTests
{
  [Theory]
  [LexerCorrectData]
  [LexerErrorData]
  internal void It_ShouldReturnExpectedTokens(string document, IEnumerable<Token> expectedTokens)
  {
    // Arrange
    List<Token> tokens = new();
    GraphQLLexer sut = new(document);

    // Act
    while (sut.MoveNext())
    {
      TokenKind kind = sut.TokenKind;
      string value = sut.Value.ToString();

      tokens.Add(new(kind, value));
    }

    // Assert
    tokens.Should().BeEquivalentTo(expectedTokens);
  }

  private class LexerCorrectDataAttribute : DataAttribute
  {
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
      yield return SingleToken("?", TokenKind.QuestionMark); // Question mark

      yield return SingleToken("&", TokenKind.Ampersand); // Ampersand

      yield return SingleToken("[", TokenKind.LeftBracket); // Left bracket

      yield return SingleToken("]", TokenKind.RightBracket); // Right bracket

      yield return SingleToken("=", TokenKind.Equals); // Equals

      yield return SingleToken("|", TokenKind.Pipe); // Pipe

      yield return SingleToken("...", TokenKind.Spread); // Spread

      yield return SingleToken("n", TokenKind.Name); // One-letter name

      yield return SingleToken("my_name", TokenKind.Name); // Multiple-letter name

      yield return SingleToken("0", TokenKind.Integer); // Zero

      yield return SingleToken("1", TokenKind.Integer); // One-digit integer

      yield return SingleToken("10", TokenKind.Integer); // Two-digit integer

      yield return SingleToken("-10", TokenKind.Integer); // Negative integer

      yield return SingleToken("12.3", TokenKind.Float); // One-digit fractionary part

      yield return SingleToken("12.34", TokenKind.Float); // Two-digit fractionary part

      yield return SingleToken("12e3", TokenKind.Float); // One-digit exponential part

      yield return SingleToken("12E34", TokenKind.Float); // Two-digit exponential part

      yield return SingleToken("12e+34", TokenKind.Float); // Exponential part with plus sign

      yield return SingleToken("12e-34", TokenKind.Float); // Exponential part with minus sign

      yield return SingleToken("12.34e56", TokenKind.Float); // Fractionary and exponential parts

      yield return SingleToken("-12.34e-56", TokenKind.Float); // Negative float

      yield return SingleToken("""
        "s"
        """, TokenKind.String); // One-character string

      yield return SingleToken("""
        "my-string"
        """, TokenKind.String); // Multiple-character string

      yield return SingleToken("""
        "😎"
        """, TokenKind.String); // String with unicode characters

      yield return SingleToken("""
        "\r hello \b  world \t"
        """, TokenKind.String); // String with escaped characters

      yield return SingleToken("""
        "\" hello \" world \""
        """, TokenKind.String); // String with escaped double quotes

      yield return SingleToken(""""
        """
        my
        block
        string
        """
        """", TokenKind.BlockString); // Block string

      yield return SingleToken(""""
        """
        \r
        hello
        \b
        world
        \t
        """
        """", TokenKind.BlockString); // Block string with escaped characters

      yield return SingleToken(""""
        """
        \"""
        hello
        \"""
        world
        \"""
        """
        """", TokenKind.BlockString); // Block string with escaped triple quotes

      yield return SingleToken("""
        ,,,
        my_name
        # comment
        """, TokenKind.Name, "my_name"); // Token with trivia
    }

    private static object[] SingleToken(string document, TokenKind kind, string? value = null)
    {
      value ??= document;
      return Tokens(document, new Token(kind, value));
    }

    private static object[] Tokens(string document, params Token[] tokens)
    {
      return new object[] { document, tokens.Append(new Token(TokenKind.EndOfFile, "")) };
    }
  }

  private class LexerErrorDataAttribute : DataAttribute
  {
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
      yield return ErrorToken("^"); // Unexpected character
      yield return ErrorToken("12."); // Missing fractional part
      yield return ErrorToken("12.e2"); // Missing fractional part
      yield return ErrorToken("12.3e"); // Missing exponential part
      yield return ErrorToken("12.3e-"); // Missing exponential part
      yield return ErrorToken("12.3e-"); // Missing exponential part
      yield return ErrorToken("01"); // Digit after zero
      yield return ErrorToken("0a"); // Letter after zero
      yield return ErrorToken("-01"); // Digit after negative zero
      yield return ErrorToken("-0a"); // Letter after negative zero
      yield return ErrorToken("00"); // Double zero
      yield return ErrorToken("-00"); // Negative double zero
      yield return ErrorToken("-"); // Lonely minus sign
      yield return ErrorToken("-a"); // Letter after minus sign
      yield return ErrorToken("-ab"); // Letters after minus sign
      yield return ErrorToken("1ee2"); // Number with double 'e'
      yield return ErrorToken("1e.e2"); // Number with double 'e'
      yield return ErrorToken("""
        "unterminated
        """); // Unterminated string (EOF)
      yield return ErrorToken("""
        "unterminated

        """, """
        "unterminated
        """); // Unterminated string (newline)
      yield return ErrorToken(""""
        """
        unterminated
        """"); // Unterminated block string
    }

    private static object[] ErrorToken(string document, string? value = null)
    {
      value ??= document;
      return new object[] { document, new[] { new Token(TokenKind.Error, value), new Token(TokenKind.EndOfFile, "") } };
    }
  }

  internal record struct Token(TokenKind Kind, string Value);
}
