using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : ITypeConditionNode
{
  INamedTypeNode ITypeConditionNode.Type => this;
}
