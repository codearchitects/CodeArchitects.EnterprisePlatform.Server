using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class ExpressionObjectFieldNode : IObjectFieldNode
{
  private readonly INodeRoot _root;
  private readonly string? _name;
  private readonly Expression _value;

  public ExpressionObjectFieldNode(INodeRoot root, string? name, Expression value)
  {
    _root = root;
    _name = name;
    _value = value;
  }

  public ReadOnlySpan<char> Name => _name;

  public IValueNode Value => NodeFactory.CreateValue(_root, _value);
}
