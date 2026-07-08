namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IFloatValueNode : IValueNode
{
  double Value { get; }
}
