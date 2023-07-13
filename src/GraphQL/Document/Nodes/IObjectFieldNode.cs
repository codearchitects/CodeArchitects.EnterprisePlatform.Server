namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

internal interface IObjectFieldNode
{
  string Name { get; }

  object? Value { get; }
}