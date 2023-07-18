using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class ObjectFieldNode : IObjectFieldNode
{
  private readonly string? _name;

  public ObjectFieldNode(string name, object? value)
  {
    _name = name;
    Value = value;
  }

  public ReadOnlySpan<char> Name => _name;

  public object? Value { get; }
}
