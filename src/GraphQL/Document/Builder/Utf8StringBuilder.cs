using CodeArchitects.Platform.Common.Exceptions;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

[DebuggerDisplay("{ToString(),nq}")]
internal readonly ref struct Utf8StringBuilder
{
  private const int s_maxIntFormatLength = 11;
  private const int s_maxFloatFormatLength = 128;
  private const int s_spaceBufferCharCount = 32;

  private static readonly byte[] s_newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
  private static readonly byte[] s_spaceBytes   = Encoding.UTF8.GetBytes(new string(' ', s_spaceBufferCharCount));

  private readonly ArrayBufferWriter<byte> _writer;

  public Utf8StringBuilder(ArrayBufferWriter<byte> buffer)
  {
    _writer = buffer;
  }

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
    Span<byte> span = _writer.GetSpan(s_maxFloatFormatLength);

    bool success = Utf8Formatter.TryFormat(value, span, out int bytesWritten);
    Debug.Assert(success);

    _writer.Advance(bytesWritten);

    return this;
  }

  public Utf8StringBuilder AppendLiteral(int value)
  {
    Span<byte> span = _writer.GetSpan(s_maxIntFormatLength);

    bool success = Utf8Formatter.TryFormat(value, span, out int bytesWritten);
    Debug.Assert(success);

    _writer.Advance(bytesWritten);

    return this;
  }

  public Utf8StringBuilder AppendLiteral(bool value)
  {
    return Append(value ? "true"u8 : "false"u8);
  }

  public Utf8StringBuilder AppendLiteral(string value)
  {
    Append("\""u8);
    Append(value.AsSpan());
    Append("\""u8);

    return this;
  }

  public Utf8StringBuilder AppendSpace() => Append(" "u8);

  public Utf8StringBuilder AppendSpaces(int spaceCount)
  {
    while (spaceCount > s_spaceBufferCharCount)
    {
      Append(s_spaceBytes);
      spaceCount -= s_spaceBufferCharCount;
    }

    Append(s_spaceBytes[..spaceCount]);

    return this;
  }

  public Utf8StringBuilder AppendOperationType(OperationType type)
  {
    return Append(type switch
    {
      OperationType.Query        => "query"u8,
      OperationType.Mutation     => "mutation"u8,
      OperationType.Subscription => "subscription"u8,
      _                          => throw Errors.Unreachable,
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

  [ExcludeFromCodeCoverage]
  public override string ToString() => Encoding.UTF8.GetString(_writer.WrittenSpan);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Utf8StringBuilder Append(ReadOnlySpan<byte> bytes)
  {
    int length = bytes.Length;
    Span<byte> span = _writer.GetSpan(length);

    bytes.CopyTo(span);
    _writer.Advance(length);

    return this;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Utf8StringBuilder Append(ReadOnlySpan<char> @string)
  {
    int maxLength = Encoding.UTF8.GetMaxByteCount(@string.Length);
    int length = _writer.FreeCapacity >= maxLength
      ? maxLength
      : Encoding.UTF8.GetByteCount(@string);

    Span<byte> span = _writer.GetSpan(length);

    int bytesWritten = Encoding.UTF8.GetBytes(@string, span);
    _writer.Advance(bytesWritten);

    return this;
  }
}
