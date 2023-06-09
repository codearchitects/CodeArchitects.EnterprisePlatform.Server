using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;

internal class ExpandSelectionNode : IteratorNode, ISelectionNode,
  IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>,
  IEnumerable<IArgumentNode>, IEnumerator<IArgumentNode>
{
  private readonly INodeContext _context;

  public ExpandSelectionNode(INodeContext context, string? alias, string fieldName, Expression expression)
  {
    _context = context;
    Alias = alias;
    FieldName = fieldName;
    Expression = expression;
  }

  protected override Expression Expression { get; }

  public string? Alias { get; }

  public string FieldName { get; }

  public IEnumerable<IArgumentNode> Arguments => this;

  public IEnumerable<IDirectiveNode> Directives => this;

  public ISelectionSetNode? SelectionSet
  {
    get
    {
      if (TryGetNext(MethodNames.WithSelection, out ISelectionSetNode? selectionSet))
        return selectionSet;

      Type fieldType = Expression.Type.GetGenericArguments()[0];
      if (_context.TryGetDefaultSelection(fieldType, out LambdaExpression? defaultSelection))
        return new NamedSelectionSetNode(_context, defaultSelection.Parameters[0], (MemberInitExpression)defaultSelection.Body);

      return null;
    }
  }

  IArgumentNode IEnumerator<IArgumentNode>.Current => GetCurrent<IArgumentNode>();

  IDirectiveNode IEnumerator<IDirectiveNode>.Current => GetCurrent<IDirectiveNode>();

  IEnumerator<IArgumentNode> IEnumerable<IArgumentNode>.GetEnumerator() => GetEnumerator(MethodNames.WithArgument, this);

  IEnumerator<IDirectiveNode> IEnumerable<IDirectiveNode>.GetEnumerator() => GetEnumerator(MethodNames.WithDirective, this);

  protected override object OnMethodCall(MethodCallExpression methodCall)
  {
    return methodCall.Method.Name switch
    {
      MethodNames.WithArgument  => NodeFactory.CreateArgument(_context, methodCall),
      MethodNames.WithDirective => NodeFactory.CreateDirective(_context, methodCall),
      MethodNames.WithSelection => NodeFactory.CreateSelectionSet(_context, methodCall),
      _                         => throw new ExpressionEvaluationException(methodCall),
    };
  }
}
