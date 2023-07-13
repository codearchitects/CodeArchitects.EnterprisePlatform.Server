using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class SelectionNode : ISelectionNode
{
  private readonly INodeContext _context;
  private readonly LambdaExpression _expression;

  public SelectionNode(INodeContext context, string? alias, string fieldName, LambdaExpression expression)
  {
    _context = context;
    Alias = alias;
    FieldName = fieldName;
    _expression = expression;
  }

  public string? Alias { get; }

  public string FieldName { get; }

  public IEnumerable<IArgumentNode> Arguments => Enumerable.Empty<IArgumentNode>();

  public IEnumerable<IDirectiveNode> Directives => Enumerable.Empty<IDirectiveNode>();

  public ISelectionSetNode? SelectionSet => new NamedSelectionSetNode(_context, _expression.Parameters[0], (MemberInitExpression)_expression.Body);
}
