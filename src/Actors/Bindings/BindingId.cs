namespace CodeArchitects.Platform.Actors.Bindings;

/// <summary>
/// Uniquely identifies a binding.
/// </summary>
public readonly struct BindingId : IEquatable<BindingId>
{
  internal readonly int _index;

  internal BindingId(int index)
  {
    _index = index;
  }

  /// <inheritdoc/>
  public bool Equals(BindingId other) => other._index == _index;

  /// <inheritdoc/>
  public override bool Equals(object? obj) => obj is BindingId other && Equals(other);

  /// <inheritdoc/>
  public override int GetHashCode() => _index;

  /// <summary>
  /// Determines whether two <see cref="BindingId"/> objects are equal.
  /// </summary>
  /// <param name="left">The first <see cref="BindingId"/> object to compare.</param>
  /// <param name="right">The second <see cref="BindingId"/> object to compare.</param>
  /// <returns><see langword="true"/> if the two <see cref="BindingId"/> objects are equal; otherwise, <see langword="false"/>.</returns>
  public static bool operator ==(BindingId left, BindingId right) => left.Equals(right);

  /// <summary>
  /// Determines whether two <see cref="BindingId"/> objects are not equal.
  /// </summary>
  /// <param name="left">The first <see cref="BindingId"/> object to compare.</param>
  /// <param name="right">The second <see cref="BindingId"/> object to compare.</param>
  /// <returns><see langword="true"/> if the two <see cref="BindingId"/> objects are not equal; otherwise, <see langword="false"/>.</returns>
  public static bool operator !=(BindingId left, BindingId right) => !(left == right);
}
