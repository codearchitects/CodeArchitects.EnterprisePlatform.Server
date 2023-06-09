using CodeArchitects.Platform.GraphQL.Document.Content;

namespace CodeArchitects.Platform.GraphQL.Document;

internal abstract class OperationType : IEquatable<OperationType>
{
  public static readonly OperationType Query = new QueryOperation();
  public static readonly OperationType Mutation = new MutationOperation();
  public static readonly OperationType Subscription = new SubscriptionOperation();

  private OperationType() { }

  protected abstract int Value { get; }

  internal abstract void Append<TSymbol>(IDocumentContentBuilder<TSymbol> content);

  public bool Equals(OperationType other) => other.Value == Value;

  public sealed override bool Equals(object obj) => obj is OperationType other && Equals(other);

  public sealed override int GetHashCode() => Value;

  public abstract override string ToString();

  public static bool operator ==(OperationType left, OperationType right) => left.Equals(right);

  public static bool operator !=(OperationType left, OperationType right) => !(left == right);


  private sealed class QueryOperation : OperationType
  {
    protected override int Value => 1;

    public override string ToString() => "query";

    internal override void Append<TSymbol>(IDocumentContentBuilder<TSymbol> content)
      => content.Append(content.Keywords.Query);
  }

  private sealed class MutationOperation : OperationType
  {
    protected override int Value => 2;

    public override string ToString() => "mutation";

    internal override void Append<TSymbol>(IDocumentContentBuilder<TSymbol> content)
      => content.Append(content.Keywords.Mutation);
  }

  private sealed class SubscriptionOperation : OperationType
  {
    protected override int Value => 3;

    public override string ToString() => "subscription";

    internal override void Append<TSymbol>(IDocumentContentBuilder<TSymbol> content)
      => content.Append(content.Keywords.Subscription);
  }
}