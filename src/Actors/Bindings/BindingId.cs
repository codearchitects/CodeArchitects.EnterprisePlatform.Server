namespace CodeArchitects.Platform.Actors.Bindings;

public readonly struct BindingId : IEquatable<BindingId>
{
  internal readonly int _index;

  internal BindingId(int index)
  {
    _index = index;
  }

  public bool Equals(BindingId other)
  {
    return other._index == _index;
  }

  public override bool Equals(object obj)
  {
    return obj is BindingId other && Equals(other);
  }

  public override int GetHashCode()
  {
    return _index;
  }

  public static bool operator ==(BindingId left, BindingId right) => left.Equals(right);

  public static bool operator !=(BindingId left, BindingId right) => !(left == right);
}
