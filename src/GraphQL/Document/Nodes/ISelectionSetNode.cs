namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface ISelectionSetNode
{
  IEnumerable<ISelectionNode> Selections { get; }
}
