using System.Linq.Expressions;
using System.Reflection;
using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;

internal class AnonymousObjectValueNode : ListIterator<MemberInfo, Expression, IObjectFieldNode>, IObjectValueNode
{
  private readonly INodeContext _context;
  private readonly NewExpression _expression;

  public AnonymousObjectValueNode(INodeContext context, NewExpression expression)
  {
    _context = context;
    _expression = expression;
  }

  public IEnumerable<IObjectFieldNode> Fields => this;

  protected override IReadOnlyList<MemberInfo> List1 => _expression.Members;

  protected override IReadOnlyList<Expression> List2 => _expression.Arguments;

  protected override IObjectFieldNode OnCurrent(MemberInfo member, Expression argument)
  {
    return new ObjectFieldNode(member.Name, NodeFactory.CreateValue(_context, argument));
  }
}
