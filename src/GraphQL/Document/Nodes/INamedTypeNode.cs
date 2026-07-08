namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface INamedTypeNode : ITypeNode
{
  ReadOnlySpan<char> Name { get; }
}
