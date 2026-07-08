namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IFragmentSpreadNode : ISelectionNode
{
  ReadOnlySpan<char> FragmentName { get; }

  IDirectiveListNode? DirectiveList { get; }
}
