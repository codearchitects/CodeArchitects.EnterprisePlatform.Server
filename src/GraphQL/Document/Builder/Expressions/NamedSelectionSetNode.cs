using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;

internal class NamedSelectionSetNode : ListIterator<MemberBinding, ISelectionNode>, ISelectionSetNode
{
  private readonly INodeContext _context;
  private readonly Expression _field;
  private readonly MemberInitExpression _expression;

  public NamedSelectionSetNode(INodeContext context, Expression field, MemberInitExpression expression)
  {
    _context = context;
    _field = field;
    _expression = expression;
  }

  public IEnumerable<ISelectionNode> Selections => this;

  protected override IReadOnlyList<MemberBinding> List => _expression.Bindings;

  protected override ISelectionNode OnCurrent(MemberBinding binding)
  {
    if (binding is not MemberAssignment assignment)
      throw new ExpressionEvaluationException(_expression);

    return NodeFactory.CreateSelection(_context, _field, assignment.Member.Name, assignment.Expression);
  }
}
