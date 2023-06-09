using CodeArchitects.Platform.GraphQL.Document.Content;

namespace CodeArchitects.Platform.GraphQL.Document;

internal abstract class OperationType : IEquatable<OperationType>
{
  public static readonly OperationType Query = new QueryOperation();
  public static readonly OperationType Mutation = new MutationOperation();
  public static readonly OperationType Subscription = new SubscriptionOperation();

  private OperationType() { }

  protected abstract int Value { get; }

  internal abstract void AppendOn<TSymbol>(IDocumentContentBuilder<TSymbol> content);

  internal abstract T Match<T>(Func<T> whenQuery, Func<T> whenMutation, Func<T> whenSubscription);

  public bool Equals(OperationType other) => other.Value == Value;

  public sealed override bool Equals(object obj) => obj is OperationType other && Equals(other);

  public sealed override int GetHashCode() => Value;

  public static bool operator ==(OperationType left, OperationType right) => left.Equals(right);

  public static bool operator !=(OperationType left, OperationType right) => !(left == right);


  private sealed class QueryOperation : OperationType
  {
    protected override int Value => 1;

    public override string ToString() => "query";

    internal override void AppendOn<TSymbol>(IDocumentContentBuilder<TSymbol> content)
      => content.Append(content.Keywords.Query);

    internal override T Match<T>(Func<T> whenQuery, Func<T> whenMutation, Func<T> whenSubscription)
    {
      return whenQuery();
    }
  }

  private sealed class MutationOperation : OperationType
  {
    protected override int Value => 2;

    public override string ToString() => "mutation";

    internal override void AppendOn<TSymbol>(IDocumentContentBuilder<TSymbol> content)
      => content.Append(content.Keywords.Mutation);

    internal override T Match<T>(Func<T> whenQuery, Func<T> whenMutation, Func<T> whenSubscription)
    {
      return whenMutation();
    }
  }

  private sealed class SubscriptionOperation : OperationType
  {
    protected override int Value => 3;

    public override string ToString() => "subscription";

    internal override void AppendOn<TSymbol>(IDocumentContentBuilder<TSymbol> content)
      => content.Append(content.Keywords.Subscription);

    internal override T Match<T>(Func<T> whenQuery, Func<T> whenMutation, Func<T> whenSubscription)
    {
      return whenSubscription();
    }
  }
}