using CodeArchitects.Platform.Common.Exceptions;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal readonly struct Utf8StringBuilder
{
  private const int s_maxIntFormatLength = 11;
  private const int s_maxFloatFormatLength = 128;
  private const int s_spaceBufferCharCount = 32;

  private static readonly byte[] s_newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
  private static readonly byte[] s_spaceBytes = Encoding.UTF8.GetBytes(new string(' ', s_spaceBufferCharCount));

  private readonly Stream _ms;

  public Utf8StringBuilder(Stream ms)
  {
    _ms = ms;
  }

  public long Length => _ms.Length;

  public Utf8StringBuilder Append(string @string)
  {
    return Append(@string.AsSpan());
  }

  public Utf8StringBuilder AppendCamelized(string @string)
  {
    Append(stackalloc char[1] { char.ToLowerInvariant(@string[0]) });
    Append(@string.AsSpan()[1..]);

    return this;
  }

  public Utf8StringBuilder AppendLiteral(float value)
  {
    Span<byte> span = stackalloc byte[s_maxFloatFormatLength];

    bool success = Utf8Formatter.TryFormat(value, span, out int bytesWritten);
    Debug.Assert(success);

    _ms.Write(span[..bytesWritten]);

    return this;
  }

  public Utf8StringBuilder AppendLiteral(int value)
  {
    Span<byte> span = stackalloc byte[s_maxIntFormatLength];

    bool success = Utf8Formatter.TryFormat(value, span, out int bytesWritten);
    Debug.Assert(success);

    _ms.Write(span[..bytesWritten]);

    return this;
  }

  public Utf8StringBuilder AppendLiteral(bool value)
  {
    return Append(value ? "true"u8 : "false"u8);
  }

  public Utf8StringBuilder AppendLiteral(string value)
  {
    _ms.Write("\""u8);
    Append(value.AsSpan());
    _ms.Write("\""u8);

    return this;
  }

  public Utf8StringBuilder AppendSpace() => Append(" "u8);

  public Utf8StringBuilder AppendSpaces(int spaceCount)
  {
    while (spaceCount > s_spaceBufferCharCount)
    {
      _ms.Write(s_spaceBytes);
      spaceCount -= s_spaceBufferCharCount;
    }

    _ms.Write(s_spaceBytes[..spaceCount]);

    return this;
  }

  public Utf8StringBuilder AppendOperationType(OperationType type)
  {
    return Append(type switch
    {
      OperationType.Query => "query"u8,
      OperationType.Mutation => "mutation"u8,
      OperationType.Subscription => "subscription"u8,
      _ => throw Errors.Unreachable,
    });
  }

  public Utf8StringBuilder AppendNullKeyword() => Append("null"u8);

  public Utf8StringBuilder AppendDirectivePrefix() => Append("@"u8);

  public Utf8StringBuilder AppendVariablePrefix() => Append("$"u8);

  public Utf8StringBuilder AppendBang() => Append("!"u8);

  public Utf8StringBuilder AppendComma() => Append(","u8);

  public Utf8StringBuilder AppendColon() => Append(":"u8);

  public Utf8StringBuilder AppendLeftParenthesis() => Append("("u8);

  public Utf8StringBuilder AppendRightParenthesis() => Append(")"u8);

  public Utf8StringBuilder AppendLeftBracket() => Append("["u8);

  public Utf8StringBuilder AppendRightBracket() => Append("]"u8);

  public Utf8StringBuilder AppendLeftBrace() => Append("{"u8);

  public Utf8StringBuilder AppendRightBrace() => Append("}"u8);

  public Utf8StringBuilder AppendLine() => Append(s_newLineBytes);

  public void Pop()
  {
    _ms.SetLength(_ms.Length - 1);
  }

  private Utf8StringBuilder Append(ReadOnlySpan<byte> bytes)
  {
    _ms.Write(bytes);

    return this;
  }

  private Utf8StringBuilder Append(ReadOnlySpan<char> @string)
  {
    int byteCount = Encoding.UTF8.GetByteCount(@string);

    if (byteCount <= 256)
    {
      AppendUsingSpan(@string, byteCount);
    }
    else
    {
      AppendUsingArray(@string, byteCount);
    }

    return this;
  }

  private void AppendUsingSpan(ReadOnlySpan<char> @string, int byteCount)
  {
    Span<byte> bytes = stackalloc byte[byteCount];
    Encoding.UTF8.GetBytes(@string, bytes);
    _ms.Write(bytes);
  }

  private void AppendUsingArray(ReadOnlySpan<char> @string, int byteCount)
  {
    byte[] bytes = ArrayPool<byte>.Shared.Rent(byteCount);
    Encoding.UTF8.GetBytes(@string, bytes);
    _ms.Write(bytes);

    ArrayPool<byte>.Shared.Return(bytes);
  }
}
