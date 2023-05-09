using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Common;

/// <summary>
/// Combines a value, <see cref="Value"/>, and a flag, <see cref="HasValue"/>, indicating whether or not that value is meaningful.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
[DebuggerDisplay("{DebuggerDisplay}")]
internal readonly struct Optional<T> : IEquatable<Optional<T>>
{
  /// <summary>
  /// Constructs an <see cref="Optional{T}"/> with a meaningful value.
  /// </summary>
  /// <param name="value">The value.</param>
  public Optional(T value)
  {
    HasValue = true;
    Value = value;
  }

  /// <summary>
  /// Indicates whether the <see cref="Value"/> is meaningful.
  /// </summary>
  public bool HasValue { get; }

  /// <summary>
  /// Gets the value of the current object. Not meaningful unless <see cref="HasValue"/> returns <see langword="true"/>.
  /// </summary>
  /// <remarks>
  /// Unlike <see cref="Nullable{T}.Value"/>, this property does not throw an exception when <see cref="HasValue"/> is <see langword="false"/>.
  /// </remarks>
  /// <returns>
  /// <para>The value if <see cref="HasValue"/> is <see langword="true"/>; otherwise, the default value for type <typeparamref name="T"/>.</para>
  /// </returns>
  public T Value { get; }

  /// <summary>
  /// Gets an optional with no value.
  /// </summary>
  public static Optional<T> None => default;

  /// <inheritdoc/>
  public bool Equals(Optional<T> other)
  {
    if (HasValue)
    {
      if (!other.HasValue)
        return false;

      if (Value is null)
        return other.Value is null;

      return Value.Equals(other.Value);
    }

    return !other.HasValue;
  }

  /// <inheritdoc/>
  public override bool Equals(object? obj)
  {
    return obj is Optional<T> other && Equals(other);
  }

  /// <inheritdoc/>
  public override string? ToString()
  {
    return HasValue ? Value?.ToString() : "";
  }

  /// <inheritdoc/>
  public override int GetHashCode()
  {
    return HasValue
      ? Value is { } value
        ? value.GetHashCode()
        : typeof(T).GetHashCode()
      : 0;
  }

  /// <summary>
  /// Determines whether the specified objects are equal.
  /// </summary>
  /// <param name="left">The first object to compare.</param>
  /// <param name="right">The second object to compare.</param>
  /// <returns><see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.</returns>
  public static bool operator ==(Optional<T> left, Optional<T> right)
  {
    return left.Equals(right);
  }

  /// <summary>
  /// Determines whether the specified objects are not equal.
  /// </summary>
  /// <param name="left">The first object to compare.</param>
  /// <param name="right">The second object to compare.</param>
  /// <returns><see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.</returns>
  public static bool operator !=(Optional<T> left, Optional<T> right)
  {
    return !(left == right);
  }

  /// <summary>
  /// Implicitly converts a value to an optional.
  /// </summary>
  /// <param name="value">The value to convert.</param>
  public static implicit operator Optional<T>(T value)
  {
    return new Optional<T>(value);
  }

  #region Debugger display

  [ExcludeFromCodeCoverage]
  [DebuggerBrowsable(DebuggerBrowsableState.Never)]
  private object DebuggerDisplay
  {
    get
    {
      if (!HasValue)
        return NoneValue.Instance;

      if (Value is null)
        return NullValue.Instance;

      return Value;
    }
  }

  [ExcludeFromCodeCoverage]
  [DebuggerDisplay("<none>")]
  private sealed class NoneValue
  {
    public static readonly NoneValue Instance = new();

    private NoneValue() { }
  }

  [ExcludeFromCodeCoverage]
  [DebuggerDisplay("null")]
  private sealed class NullValue
  {
    public static readonly NullValue Instance = new();

    private NullValue() { }
  }

  #endregion
}
