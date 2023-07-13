using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class AnonymousSelectionSetNode : ListIterator<MemberInfo, Expression, ISelectionNode>, ISelectionSetNode,
  IEnumerable<ISelectionNode>, IEnumerator<ISelectionNode>
{
  private readonly INodeContext _context;
  private readonly Expression _field;
  private readonly NewExpression _expression;

  public AnonymousSelectionSetNode(INodeContext context, Expression field, NewExpression expression)
  {
    _context = context;
    _field = field;
    _expression = expression;
  }

  public IEnumerable<ISelectionNode> Selections => this;

  protected override IReadOnlyList<MemberInfo> List1 => _expression.Members;

  protected override IReadOnlyList<Expression> List2 => _expression.Arguments;

  protected override ISelectionNode OnCurrent(MemberInfo member, Expression argument)
  {
    return NodeFactory.CreateSelection(_context, _field, member.Name, argument);
  }
}
