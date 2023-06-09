namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

internal interface IListValueNode
{
  IEnumerable<object?> Values { get; }
}
