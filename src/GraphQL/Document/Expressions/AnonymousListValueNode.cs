using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class AnonymousListValueNode : ListIterator<Expression, object?>, IListValueNode
{
  private readonly INodeRoot _root;
  private readonly NewArrayExpression _expression;

  public AnonymousListValueNode(INodeRoot root, NewArrayExpression expression)
  {
    _root = root;
    _expression = expression;
  }

  public IEnumerable<object?> Values => this;

  protected override IReadOnlyList<Expression> List => _expression.Expressions;

  protected override object? OnCurrent(Expression element)
  {
    return NodeFactory.CreateValue(_root, element);
  }
}
