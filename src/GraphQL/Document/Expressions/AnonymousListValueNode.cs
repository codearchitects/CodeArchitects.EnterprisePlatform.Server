using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class AnonymousListValueNode : ListIterator<Expression, IValueNode>, IListValueNode
{
  private readonly INodeRoot _root;
  private readonly NewArrayExpression _expression;

  public AnonymousListValueNode(INodeRoot root, NewArrayExpression expression)
  {
    _root = root;
    _expression = expression;
  }

  public ValueNodeKind ValueKind => ValueNodeKind.ListValue;

  public IEnumerable<IValueNode> Values => this;

  protected override IReadOnlyList<Expression> List => _expression.Expressions;

  protected override IValueNode OnCurrent(Expression element)
  {
    return NodeFactory.CreateValue(_root, element);
  }
}
