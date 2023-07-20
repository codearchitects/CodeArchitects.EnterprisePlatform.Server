using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class AnonymousObjectValueNode : ListIterator<MemberInfo, Expression, IObjectFieldNode>, IObjectValueNode
{
  private readonly INodeRoot _root;
  private readonly NewExpression _expression;

  public AnonymousObjectValueNode(INodeRoot root, NewExpression expression)
  {
    _root = root;
    _expression = expression;
  }

  public IEnumerable<IObjectFieldNode> Fields => this;

  protected override IReadOnlyList<MemberInfo> List1 => _expression.Members;

  protected override IReadOnlyList<Expression> List2 => _expression.Arguments;

  protected override IObjectFieldNode OnCurrent(MemberInfo member, Expression argument)
  {
    return new ObjectFieldNode(member.Name, NodeFactory.CreateValue(_root, argument));
  }
}
