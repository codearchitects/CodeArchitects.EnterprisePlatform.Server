namespace CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

internal interface IArgumentNode
{
  string Name { get; }

  object? Value { get; }
}