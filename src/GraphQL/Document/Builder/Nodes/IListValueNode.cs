namespace CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

internal interface IListValueNode
{
  IEnumerable<object?> Values { get; }
}
