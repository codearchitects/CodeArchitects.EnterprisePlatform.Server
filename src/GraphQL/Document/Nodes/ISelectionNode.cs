namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface ISelectionNode
{
  SelectionNodeKind SelectionKind { get; }
}
