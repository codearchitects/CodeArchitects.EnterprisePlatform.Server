using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.GraphQL.Document.Syntax;

[DebuggerDisplay("Current: {TokenKind}")]
internal struct GraphQLLexer
{
  private readonly string _document;
  private TokenKind _kind;
  private int _start;
  private int _position;
  private string? _error;

  public GraphQLLexer(string document)
  {
    _document = document;
    _kind = TokenKind.StartOfFile;
  }

  public readonly string? Error => _error;

  public readonly TokenKind TokenKind => _kind;

  public readonly ReadOnlySpan<char> Value => _document.AsSpan(_start, _position - _start);

  private readonly char Current => _document[_position];

  public bool MoveNext()
  {
    if (_kind is TokenKind.EndOfFile)
      return false;

    _start = _position;
    _error = null;

    if (IsAtEnd())
    {
      _kind = TokenKind.EndOfFile;
      return true;
    }

    char current = Current;
    Advance();
    switch (current)
    {
      case '!':
        _kind = TokenKind.Bang;
        break;

      case '?':
        _kind = TokenKind.QuestionMark;
        break;

      case '$':
        _kind = TokenKind.Dollar;
        break;

      case '&':
        _kind = TokenKind.Ampersand;
        break;

      case '(':
        _kind = TokenKind.LeftParenthesis;
        break;

      case ')':
        _kind = TokenKind.RightParenthesis;
        break;

      case '[':
        _kind = TokenKind.LeftBracket;
        break;

      case ']':
        _kind = TokenKind.RightBracket;
        break;

      case '{':
        _kind = TokenKind.LeftBrace;
        break;

      case '}':
        _kind = TokenKind.RightBrace;
        break;

      case ':':
        _kind = TokenKind.Colon;
        break;

      case '=':
        _kind = TokenKind.Equals;
        break;

      case '@':
        _kind = TokenKind.At;
        break;

      case '|':
        _kind = TokenKind.Pipe;
        break;

      case '.' when Match(".."):
        _kind = TokenKind.Spread;
        break;

      case >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_':
        Name();
        break;

      case '"' when Match("\"\""):
        BlockString();
        break;

      case '"':
        String();
        break;

      case >= '1' and <= '9':
        NonZeroDigit();
        break;

      case '-':
        MinusSign();
        break;

      case '0':
        Zero();
        break;

      case ' ' or '\t' or '\n' or '\r' or ',':
        Ignored();
        return MoveNext();

      case '#':
        Comment();
        return MoveNext();

      default:
        _kind = TokenKind.Error;
        _error = $"Unexpected character '{current}'";
        break;
    }

    return true;
  }

  private void Name()
  {
    _kind = TokenKind.Name;

    MatchWhile(IsAlphaNumeric);
  }

  private void String()
  {
    while (!IsAtEnd())
    {
      char current = Current;

      if (IsNewlineCharacter(current))
        break;

      Advance();
      if (IsDoubleQuote(current))
      {
        _kind = TokenKind.String;
        return;
      }

      if (IsBackSlash(current))
      {
        Match('"');
        continue;
      }
    }

    _kind = TokenKind.Error;
    _error = "Unterminated string";
  }

  private void BlockString()
  {
    while (!IsAtEnd())
    {
      char current = Current;
      Advance();

      if (IsDoubleQuote(current) && Match("\"\""))
      {
        _kind = TokenKind.BlockString;
        return;
      }

      if (IsBackSlash(current))
      {
        Match("\"\"\"");
        continue;
      }
    }

    _kind = TokenKind.Error;
    _error = "Unterminated block string";
  }

  private void NonZeroDigit()
  {
    _kind = TokenKind.Integer;

    MatchWhile(IsDigit);

    CheckFloat();

    if (MatchWhile(IsWordLike))
    {
      if (_kind is not TokenKind.Error)
      {
        _kind = TokenKind.Error;
        _error = "Expected digit";
      }
      return;
    }
  }

  private void MinusSign()
  {
    if (!IsAtEnd())
    {
      char current = Current;
      Advance();

      if (IsZero(current))
      {
        Zero();
        return;
      }

      if (IsDigit(current))
      {
        NonZeroDigit();
        return;
      }

      MatchWhile(IsWordLike);
    }

    _kind = TokenKind.Error;
    _error = "Expected digit";
  }

  private void Zero()
  {
    _kind = TokenKind.Integer;

    CheckFloat();

    if (_kind is TokenKind.Integer && MatchWhile(IsDigit))
    {
      _kind = TokenKind.Error;
      _error = "Unexpected digit";
      return;
    }

    if (MatchWhile(IsWordLike))
    {
      if (_kind is not TokenKind.Error)
      {
        _kind = TokenKind.Error;
        _error = "Expected digit";
      }
      return;
    }
  }

  private void CheckFloat()
  {
    if (Match('.'))
    {
      _kind = TokenKind.Float;

      if (!MatchWhile(IsDigit))
      {
        _kind = TokenKind.Error;
        _error = "Expected digit";
        return;
      }
    }

    if (Match(IsExponentIndicator))
    {
      _kind = TokenKind.Float;

      Match(IsSign);
      if (!MatchWhile(IsDigit))
      {
        _kind = TokenKind.Error;
        _error = "Expected digit";
      }
    }
  }

  private void Ignored()
  {
    MatchWhile(IsIgnored);
  }

  private void Comment()
  {
    MatchUntil(IsNewlineCharacter);
  }

  private void Advance()
  {
    _position++;
  }

  private void Advance(int distance)
  {
    _position += distance;
  }

  private bool Match(char expected)
  {
    if (IsAtEnd())
      return false;

    if (Current != expected)
      return false;

    Advance();
    return true;
  }

  private bool Match(string expected)
  {
    int distance = expected.Length;

    if (IsAtEnd(distance - 1))
      return false;

    for (int i = 0; i < distance; i++)
    {
      if (Peek(i) != expected[i])
        return false;
    }

    Advance(distance);
    return true;
  }

  private bool Match(Predicate<char> predicate)
  {
    if (IsAtEnd())
      return false;

    if (!predicate(Current))
      return false;

    Advance();
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private bool MatchWhile(Predicate<char> predicate) => MatchWhile(predicate, out _);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private bool MatchUntil(Predicate<char> predicate) => MatchUntil(predicate, out _);

  private bool MatchWhile(Predicate<char> predicate, out char current)
  {
    if (IsAtEnd())
    {
      current = '\0';
      return false;
    }

    if (!predicate(current = Current))
      return false;

    do
    {
      Advance();

      if (IsAtEnd())
      {
        current = '\0';
        break;
      }

      if (!predicate(current = Current))
        break;
    }
    while (true);

    return true;
  }

  private bool MatchUntil(Predicate<char> predicate, out char current)
  {
    if (IsAtEnd())
    {
      current = '\0';
      return false;
    }

    if (predicate(current = Current))
      return false;

    do
    {
      Advance();

      if (IsAtEnd())
      {
        current = '\0';
        break;
      }

      if (predicate(current = Current))
        break;
    }
    while (true);

    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private readonly char Peek(int distance)
  {
    return _document[_position + distance];
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private readonly bool IsAtEnd()
  {
    return _position >= _document.Length;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private readonly bool IsAtEnd(int distance)
  {
    return _position + distance >= _document.Length;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsAlpha(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsDigit(char c) => c is >= '0' and <= '9';

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsZero(char c) => c is '0';

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsIgnored(char c) => c is ' ' or '\t' or ',' || IsNewlineCharacter(c);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsNewlineCharacter(char c) => c is '\n' or '\r';

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsSign(char c) => c is '+' or '-';

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsWordLike(char c) => IsAlphaNumeric(c) || IsSign(c) || c is '.';

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsExponentIndicator(char c) => c is 'e' or 'E';

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsBackSlash(char c) => c is '\\';

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsDoubleQuote(char c) => c is '"';
}
