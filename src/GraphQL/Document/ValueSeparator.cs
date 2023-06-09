namespace CodeArchitects.Platform.GraphQL.Document;

internal sealed class ValueSeparator : IEquatable<ValueSeparator>
{
  public static readonly ValueSeparator Comma = new(", ");
  public static readonly ValueSeparator Space = new(" ");

  private readonly string _separator;

  public ValueSeparator(string separator)
  {
    _separator = separator;
  }

  public bool Equals(ValueSeparator other) => other._separator == _separator;

  public override bool Equals(object obj) => obj is ValueSeparator other && Equals(other);

  public override int GetHashCode() => _separator.GetHashCode();

  public override string ToString() => _separator;

  public static bool operator ==(ValueSeparator left, ValueSeparator right) => left.Equals(right);

  public static bool operator !=(ValueSeparator left, ValueSeparator right) => !(left == right);
}
