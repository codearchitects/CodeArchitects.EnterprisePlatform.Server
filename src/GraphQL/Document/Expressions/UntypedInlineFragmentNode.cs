using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class UntypedInlineFragmentNode : InlineFragmentNode
{
  public UntypedInlineFragmentNode(INodeRoot root, Expression expression)
    : base(root, expression)
  {
  }

  public override ITypeConditionNode? TypeCondition => null;
}
