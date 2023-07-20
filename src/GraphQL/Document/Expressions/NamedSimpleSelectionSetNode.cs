using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class NamedSimpleSelectionSetNode : ListIterator<MemberBinding, ISelectionNode>, ISelectionSetNode
{
  private readonly INodeRoot _root;
  private readonly Expression _field;
  private readonly MemberInitExpression _expression;

  public NamedSimpleSelectionSetNode(INodeRoot root, Expression field, MemberInitExpression expression)
  {
    _root = root;
    _field = field;
    _expression = expression;
  }

  public IEnumerable<ISelectionNode> Selections => this;

  protected override IReadOnlyList<MemberBinding> List => _expression.Bindings;

  protected override ISelectionNode OnCurrent(MemberBinding binding)
  {
    if (binding is not MemberAssignment assignment)
      throw new ExpressionEvaluationException(_expression);

    return NodeFactory.CreateField(_root, _field, assignment.Member.Name, assignment.Expression);
  }
}
