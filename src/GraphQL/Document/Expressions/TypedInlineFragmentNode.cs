using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class TypedInlineFragmentNode : InlineFragmentNode
{
  private readonly INamedType _fragmentType;

  public TypedInlineFragmentNode(INodeRoot root, Expression expression, INamedType fragmentType)
    : base(root, expression)
  {
    _fragmentType = fragmentType;
  }

  public override ITypeConditionNode? TypeCondition => new TypeConditionNode(_fragmentType);
}
