using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class FragmentDefinitionNode : IteratorNode, INodeRoot, IFragmentDefinitionNode,
  IDirectiveListNode, IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>
{
  private readonly INodeContext _context;
  private readonly string _name;
  private readonly INamedType _type;

  public FragmentDefinitionNode(INodeContext context, string name, INamedType type, Expression expression)
  {
    _context = context;
    _name = name;
    _type = type;
    Expression = expression;
  }

  protected override Expression Expression { get; }

  INodeContext INodeRoot.Context => _context;

  public DefinitionNodeKind DefinitionKind => DefinitionNodeKind.FragmentDefinition;

  public ReadOnlySpan<char> FragmentName => _name;

  public ITypeConditionNode TypeCondition => new TypeConditionNode(_type);

  public IDirectiveListNode? DirectiveList => Peek(MethodNames.IsDirective) ? this : null;

  public ISelectionSetNode SelectionSet => GetNext<ISelectionSetNode>(MethodNames.IsSelectionSet);

  IDirectiveNode IEnumerator<IDirectiveNode>.Current => GetCurrent<IDirectiveNode>();

  public IEnumerable<IDirectiveNode> Directives => this;

  IEnumerator<IDirectiveNode> IEnumerable<IDirectiveNode>.GetEnumerator() => GetEnumerator(MethodNames.IsDirective, this);

  protected override object OnMethodCall(MethodCallExpression methodCall)
  {
    return methodCall.Method.Name switch
    {
      MethodNames.WithSelection    => NodeFactory.CreateSimpleSelectionSet(this, methodCall),
      MethodNames.WithSelectionSet => NodeFactory.CreateSelectionSet(this, methodCall),
      MethodNames.WithDirective    => NodeFactory.CreateDirective(this, methodCall),
      _                            => throw new ExpressionEvaluationException(methodCall)
    };
  }

  void INodeRoot.ReportFragment(GraphFragment fragment)
  {
    throw new NotSupportedException("Usage of fragment spreads inside fragment definitions is not supported.");
  }
}
