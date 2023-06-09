namespace CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

internal interface IObjectFieldNode
{
  string Name { get; }

  object? Value { get; }
}