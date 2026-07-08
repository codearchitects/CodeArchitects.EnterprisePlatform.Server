using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class SimpleFragmentSpreadNode : IFragmentSpreadNode
{
  private readonly GraphFragment _fragment;

  public SimpleFragmentSpreadNode(GraphFragment fragment)
  {
    _fragment = fragment;
  }

  public SelectionNodeKind SelectionKind => SelectionNodeKind.FragmentSpread;

  public ReadOnlySpan<char> FragmentName => _fragment.Name;

  public IDirectiveListNode? DirectiveList => null;
}
