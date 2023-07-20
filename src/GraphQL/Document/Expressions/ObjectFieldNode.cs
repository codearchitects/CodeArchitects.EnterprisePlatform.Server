using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class ObjectFieldNode : IObjectFieldNode
{
  private readonly INodeRoot _root;
  private readonly string? _name;
  private readonly object? _value;

  public ObjectFieldNode(INodeRoot root, string? name, object? value)
  {
    _root = root;
    _name = name;
    _value = value;
  }

  public ReadOnlySpan<char> Name => _name;

  public IValueNode Value => NodeFactory.CreateValue(_root, _value);
}
