using CodeArchitects.Platform.GraphQL.Document.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document;

public abstract class GraphFragment
{
  private readonly Expression _expression;

  internal GraphFragment(string name, Expression expression)
  {
    if (name is "on")
      throw new ArgumentException("Unexpected fragment name 'on'.");

    Name = name;
    _expression = expression;
  }

  protected abstract Type Type { get; }

  public string Name { get; }

  internal IFragmentDefinitionNode CreateFragmentDefinition(INodeContext context)
  {
    if (!context.TryGetObjectType(Type, out IObjectType? objectType))
      throw new InvalidOperationException($"'{Type}' is not a valid object type for a fragment.");

    return new FragmentDefinitionNode(context, Name, objectType, _expression);
  }
}

public sealed class GraphFragment<TField> : GraphFragment
{
  public GraphFragment(string name, Expression expression)
    : base(name, expression)
  {
  }

  protected override Type Type => typeof(TField);
}

public sealed class GraphFragment<TField, TVariables> : GraphFragment
  where TVariables : notnull
{
  public GraphFragment(string name, Expression expression)
    : base(name, expression)
  {
  }

  protected override Type Type => typeof(TField);
}
