namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IObjectFieldNode
{
  ReadOnlySpan<char> Name { get; }

  IValueNode Value { get; }
}