namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IStringValueNode : IValueNode
{
  string Value { get; }
}
