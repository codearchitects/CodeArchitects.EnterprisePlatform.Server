using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class SelectionSetNode : StreamingNode, ISelectionSetNode,
  IEnumerable<ISelectionNode>, IEnumerator<ISelectionNode>
{
  private readonly INodeRoot _root;
  private IEnumerator<ISelectionNode>? _simpleSelectionSetEnumerator;

  public SelectionSetNode(INodeRoot root, Expression expression)
  {
    _root = root;
    Expression = expression;
  }

  protected override Expression Expression { get; }

  public IEnumerable<ISelectionNode> Selections => this;

  ISelectionNode IEnumerator<ISelectionNode>.Current
  {
    get
    {
      if (_simpleSelectionSetEnumerator is not null)
        return _simpleSelectionSetEnumerator.Current;

      return GetCurrent<ISelectionNode>();
    }
  }

  IEnumerator<ISelectionNode> IEnumerable<ISelectionNode>.GetEnumerator() => this;

  bool IEnumerator.MoveNext()
  {
    if (_simpleSelectionSetEnumerator is not null)
      return SimpleSelectionSetMoveNext(ref _simpleSelectionSetEnumerator);

    if (!Started && TryGetNext(MethodNames.IsSelectionSet, out ISelectionSetNode? selectionSet))
    {
      _simpleSelectionSetEnumerator = selectionSet.Selections.GetEnumerator();
      return SimpleSelectionSetMoveNext(ref _simpleSelectionSetEnumerator);
    }

    return MoveNext();
  }

  private bool SimpleSelectionSetMoveNext([MaybeNull] ref IEnumerator<ISelectionNode> enumerator)
  {
    if (enumerator.MoveNext())
      return true;

    enumerator = null;
    return MoveNext();
  }

  protected override object OnMethodCall(MethodCallExpression methodCall)
  {
    return methodCall.Method.Name switch
    {
      MethodNames.WithSelection      => NodeFactory.CreateSimpleSelectionSet(_root, methodCall),
      MethodNames.WithFragmentSpread => NodeFactory.CreateFragmentSpread(_root, methodCall),
      MethodNames.WithInlineFragment => NodeFactory.CreateInlineFragment(_root, methodCall),
      _                              => throw new ExpressionEvaluationException(methodCall)
    };
  }

  #region Not relevant

  [ExcludeFromCodeCoverage]
  object IEnumerator.Current => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  void IDisposable.Dispose() { }

  [ExcludeFromCodeCoverage]
  void IEnumerator.Reset() => throw new NotSupportedException();

  #endregion
}
