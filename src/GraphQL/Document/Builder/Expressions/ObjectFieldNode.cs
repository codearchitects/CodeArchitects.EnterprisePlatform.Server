using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;

internal class ObjectFieldNode : IObjectFieldNode
{
  public ObjectFieldNode(string name, object? value)
  {
    Name = name;
    Value = value;
  }

  public string Name { get; }

  public object? Value { get; }
}
