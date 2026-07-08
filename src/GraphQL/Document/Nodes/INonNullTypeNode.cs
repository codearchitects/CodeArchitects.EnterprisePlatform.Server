namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface INonNullTypeNode : ITypeNode
{
  ITypeNode NullableType { get; }
}
