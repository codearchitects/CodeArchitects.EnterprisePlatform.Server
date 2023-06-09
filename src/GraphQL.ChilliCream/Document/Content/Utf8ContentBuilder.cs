using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Content;
using StrawberryShake;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document.Content;

internal class Utf8ContentBuilder : IDocumentContentBuilder<Symbol>
{
  private const int s_maxIntFormatLength = 11;
  private const int s_maxFloatFormatLength = 128;

  private readonly MemoryStream _ms;

  public Utf8ContentBuilder(MemoryStream ms)
  {
    _ms = ms;
  }

  public int Length => (int)_ms.Length;

  public IKeywords<Symbol> Keywords => Symbols.Instance;

  public IPunctuators<Symbol> Punctuators => Symbols.Instance;

  public ITrivias<Symbol> Trivias => Symbols.Instance;

  public void Append(ReadOnlySpan<byte> bytes)
  {
    _ms.Write(bytes);
  }

  public IDocumentContentBuilder<Symbol> Append(string @string)
  {
    return Append(@string.AsSpan());
  }

  public IDocumentContentBuilder<Symbol> Append(Symbol symbol)
  {
    symbol.AppendOn(this);

    return this;
  }

  public IDocumentContentBuilder<Symbol> Append(Symbol symbol, int repeatCount)
  {
    symbol.AppendOn(this, repeatCount);

    return this;
  }

  public IDocumentContentBuilder<Symbol> AppendCamelized(string @string)
  {
    Append(stackalloc char[1] { char.ToLower(@string[0]) });
    Append(@string.AsSpan()[1..]);

    return this;
  }

  public IDocumentContentBuilder<Symbol> AppendLine()
  {
    Symbols.Instance.NewLine.AppendOn(this);

    return this;
  }

  public IDocumentContentBuilder<Symbol> AppendLiteral(float number)
  {
    Span<byte> span = stackalloc byte[s_maxFloatFormatLength];

    bool success = Utf8Formatter.TryFormat(number, span, out int bytesWritten);
    Debug.Assert(success);

    _ms.Write(span[..bytesWritten]);

    return this;
  }

  public IDocumentContentBuilder<Symbol> AppendLiteral(int number)
  {
    Span<byte> span = stackalloc byte[s_maxIntFormatLength];

    bool success = Utf8Formatter.TryFormat(number, span, out int bytesWritten);
    Debug.Assert(success);

    _ms.Write(span[..bytesWritten]);

    return this;
  }

  public IDocumentContentBuilder<Symbol> AppendLiteral(string @string)
  {
    Symbols.Instance.DoubleQuotes.AppendOn(this);
    Append(@string.AsSpan());
    Symbols.Instance.DoubleQuotes.AppendOn(this);

    return this;
  }

  public GraphDocument<TResult, TVariables> CreateDocument<TResult, TVariables>(OperationType operationType, string? name)
    where TVariables : notnull
  {
    OperationKind kind = operationType.Match(
      static () => OperationKind.Query,
      static () => OperationKind.Mutation,
      static () => OperationKind.Subscription);

    return ChilliCreamGraphDocument<TResult, TVariables>.Create(kind, name, _ms.ToArray());
  }

  public void Pop()
  {
    _ms.SetLength(_ms.Length - 1);
  }

  private IDocumentContentBuilder<Symbol> Append(ReadOnlySpan<char> @string)
  {
    byte[]? byteArray = null;
    int byteCount = Encoding.UTF8.GetByteCount(@string);
    Span<byte> bytes = byteCount <= 256
      ? stackalloc byte[byteCount]
      : (byteArray = ArrayPool<byte>.Shared.Rent(byteCount));

    Encoding.UTF8.GetBytes(@string, bytes);
    _ms.Write(bytes);

    if (byteArray is not null)
    {
      ArrayPool<byte>.Shared.Return(byteArray);
    }

    return this;
  }
}
