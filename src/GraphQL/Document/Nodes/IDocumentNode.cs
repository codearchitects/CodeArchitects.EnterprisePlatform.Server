namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IDocumentNode
{
  IEnumerable<IDefinitionNode> Definitions { get; }
}
