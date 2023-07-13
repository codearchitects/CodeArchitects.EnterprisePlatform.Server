using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class AnonymousListValueNode : ListIterator<Expression, object?>, IListValueNode
{
  private readonly INodeContext _context;
  private readonly NewArrayExpression _expression;

  public AnonymousListValueNode(INodeContext context, NewArrayExpression expression)
  {
    _context = context;
    _expression = expression;
  }

  public IEnumerable<object?> Values => this;

  protected override IReadOnlyList<Expression> List => _expression.Expressions;

  protected override object? OnCurrent(Expression element)
  {
    return NodeFactory.CreateValue(_context, element);
  }
}
