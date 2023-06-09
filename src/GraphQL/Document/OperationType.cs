namespace CodeArchitects.Platform.GraphQL.Document;

internal sealed class OperationType : IEquatable<OperationType>
{
  public static readonly OperationType Query = new("query");
  public static readonly OperationType Mutation = new("mutation");
  public static readonly OperationType Subscription = new("subscription");

  private readonly string _keyword;

  private OperationType(string keyword)
  {
    _keyword = keyword;
  }

  public bool Equals(OperationType other) => other._keyword[0] == _keyword[0];

  public override bool Equals(object? obj) => obj is OperationType other && Equals(other);

  public override int GetHashCode() => _keyword.GetHashCode();

  public override string ToString() => _keyword;

  public static bool operator ==(OperationType left, OperationType right) => left.Equals(right);

  public static bool operator !=(OperationType left, OperationType right) => !(left == right);
}
