namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IListTypeNode : ITypeNode
{
  ITypeNode ItemType { get; }
}
