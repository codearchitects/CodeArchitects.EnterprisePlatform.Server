using CodeArchitects.Platform.GraphQL.Document.Content;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document.Content;

internal class Symbols : IKeywords<Symbol>, IPunctuators<Symbol>, ITrivias<Symbol>
{
  public static readonly Symbols Instance = new();

  private Symbols() { }

  public Symbol Query { get; } = new QuerySymbol();

  public Symbol Mutation { get; } = new MutationSymbol();

  public Symbol Subscription { get; } = new SubscriptionSymbol();

  public Symbol Null { get; } = new NullSymbol();

  public Symbol True { get; } = new TrueSymbol();

  public Symbol False { get; } = new FalseSymbol();

  public Symbol Bang { get; } = new BangSymbol();

  public Symbol Colon { get; } = new ColonSymbol();

  public Symbol DollarSign { get; } = new DollarSignSymbol();

  public Symbol AtSign { get; } = new AtSignSymbol();

  public Symbol LeftParenthesis { get; } = new LeftParenthesisSymbol();

  public Symbol RightParenthesis { get; } = new RightParenthesisSymbol();

  public Symbol LeftBracket { get; } = new LeftBracketSymbol();

  public Symbol RightBracket { get; } = new RightBracketSymbol();

  public Symbol LeftBrace { get; } = new LeftBraceSymbol();

  public Symbol RightBrace { get; } = new RightBraceSymbol();

  public Symbol Space { get; } = new SpaceSymbol();

  public Symbol Comma { get; } = new CommaSymbol();

  public Symbol DoubleQuotes { get; } = new DoubleQuotesSymbol();

  public Symbol NewLine { get; } = new NewLineSymbol();

  private sealed class QuerySymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "query"u8;
  }

  private sealed class MutationSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "mutation"u8;
  }

  private sealed class SubscriptionSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "subscription"u8;
  }

  private sealed class NullSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "null"u8;
  }

  private sealed class TrueSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "true"u8;
  }

  private sealed class FalseSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "false"u8;
  }

  private sealed class BangSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "!"u8;
  }

  private sealed class ColonSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => ":"u8;
  }

  private sealed class DollarSignSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "$"u8;
  }

  private sealed class AtSignSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "@"u8;
  }

  private sealed class LeftParenthesisSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "("u8;
  }

  private sealed class RightParenthesisSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => ")"u8;
  }

  private sealed class LeftBracketSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "["u8;
  }

  private sealed class RightBracketSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "]"u8;
  }

  private sealed class LeftBraceSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "{"u8;
  }

  private sealed class RightBraceSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "}"u8;
  }

  private sealed class CommaSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => ","u8;
  }

  private sealed class DoubleQuotesSymbol : Utf8Symbol
  {
    protected override ReadOnlySpan<byte> Bytes => "\""u8;
  }

  private sealed class NewLineSymbol : Utf8Symbol
  {
    private static readonly byte[] s_bytes = Encoding.UTF8.GetBytes(Environment.NewLine);

    protected override ReadOnlySpan<byte> Bytes => s_bytes;
  }

  private abstract class Utf8Symbol : Symbol // Avoids allocating a byte array for each symbol
  {
    protected abstract ReadOnlySpan<byte> Bytes { get; }

    public sealed override void Append(Utf8ContentBuilder content)
    {
      content.Append(Bytes);
    }

    public sealed override void Append(Utf8ContentBuilder content, int repeatCount)
    {
      throw new InvalidOperationException($"Cannot append '{GetType().Name}' repeatedly.");
    }
  }

  private sealed class SpaceSymbol : Symbol
  {
    private const int s_repeatCount = 32;
    private readonly byte[] _bytes;

    public SpaceSymbol()
    {
      _bytes = Encoding.UTF8.GetBytes(new string(' ', s_repeatCount));
    }

    public override void Append(Utf8ContentBuilder content)
    {
      content.Append(_bytes[..1]);
    }

    public override void Append(Utf8ContentBuilder content, int repeatCount)
    {
      while (repeatCount > s_repeatCount)
      {
        content.Append(_bytes);
        repeatCount -= s_repeatCount;
      }

      content.Append(_bytes[..repeatCount]);
    }
  }
}
