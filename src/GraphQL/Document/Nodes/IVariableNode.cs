namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IVariableNode : IValueNode
{
  ReadOnlySpan<char> Name { get; }
}
