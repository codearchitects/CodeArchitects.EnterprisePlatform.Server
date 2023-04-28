using CodeArchitects.Platform.Common.Utils;

namespace CodeArchitects.Platform.Data.Features.Concurrency;

/// <summary>
/// Default implementation of <see cref="IConcurrencyTokenProvider"/>.
/// </summary>
/// <remarks>
/// By default, can provide tokens of the following types:
/// <list type="bullet">
/// <item><see cref="byte"/></item>
/// <item><see cref="short"/></item>
/// <item><see cref="ushort"/></item>
/// <item><see cref="int"/></item>
/// <item><see cref="uint"/></item>
/// <item><see cref="long"/></item>
/// <item><see cref="ulong"/></item>
/// <item><see cref="string"/></item>
/// <item><see cref="Guid"/></item>
/// <item><see cref="DateTime"/></item>
/// </list>
/// </remarks>
public class ConcurrencyTokenProvider : IConcurrencyTokenProvider
{
  private readonly Dictionary<Type, ConcurrencyTokenFactory> _tokenFactories;

  /// <summary>
  /// Creates a new <see cref="ConcurrencyTokenProvider"/>.
  /// </summary>
  public ConcurrencyTokenProvider()
  {
    _tokenFactories = new()
    {
      [typeof(byte)]     = CreateByte,
      [typeof(short)]    = CreateInt16,
      [typeof(ushort)]   = CreateUInt16,
      [typeof(int)]      = CreateInt32,
      [typeof(uint)]     = CreateUInt32,
      [typeof(long)]     = CreateInt64,
      [typeof(ulong)]    = CreateUInt64,
      [typeof(string)]   = CreateString,
      [typeof(Guid)]     = CreateGuid,
      [typeof(DateTime)] = CreateDateTime
    };
  }

  /// <inheritdoc />
  public object CreateToken(Type tokenType, object? previousToken)
  {
    return _tokenFactories[tokenType].Invoke(previousToken);
  }

  /// <summary>
  /// Specifies the factory for a given concurrency token type.
  /// </summary>
  /// <param name="tokenType">The concurrency token type.</param>
  /// <param name="factory">A function that creates instances of <paramref name="tokenType"/>.</param>
  protected void UseTokenFactory(Type tokenType, ConcurrencyTokenFactory factory)
  {
    _tokenFactories[tokenType] = factory;
  }

  private static object CreateByte(object? previousToken)
  {
    return (byte)(((byte)previousToken!) + 1);
  }

  private static object CreateInt16(object? previousToken)
  {
    return (short)(((short)previousToken!) + 1);
  }

  private static object CreateUInt16(object? previousToken)
  {
    return (ushort)(((ushort)previousToken!) + 1);
  }

  private static object CreateInt32(object? previousToken)
  {
    return ((int)previousToken!) + 1;
  }

  private static object CreateUInt32(object? previousToken)
  {
    return ((uint)previousToken!) + 1;
  }

  private static object CreateInt64(object? previousToken)
  {
    return ((long)previousToken!) + 1;
  }

  private static object CreateUInt64(object? previousToken)
  {
    return ((ulong)previousToken!) + 1;
  }

  private static object CreateString(object? previousToken)
  {
    Span<byte> buffer = stackalloc byte[16];
    Rand.Instance.NextBytes(buffer);
    return Convert.ToBase64String(buffer);
  }

  private static object CreateGuid(object? previousToken)
  {
    return Guid.NewGuid();
  }

  private static object CreateDateTime(object? previousToken)
  {
    return DateTime.Now;
  }
}
