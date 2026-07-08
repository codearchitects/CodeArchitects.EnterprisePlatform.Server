using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class ArgumentNode : IArgumentNode
{
  private readonly INodeRoot _root;
  private readonly string _name;
  private readonly Expression _expression;

  public ArgumentNode(INodeRoot root, string name, Expression expression)
  {
    _root = root;
    _name = name;
    _expression = expression;
  }

  public ReadOnlySpan<char> Name => _name;

  public IValueNode Value => NodeFactory.CreateValue(_root, _expression);
}
