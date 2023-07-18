namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IVariableNode
{
  ReadOnlySpan<char> Name { get; }
}
