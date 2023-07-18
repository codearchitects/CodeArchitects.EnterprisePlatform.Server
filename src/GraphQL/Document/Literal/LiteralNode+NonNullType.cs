using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : INonNullTypeNode
{
  ITypeNode INonNullTypeNode.NullableType => this;
}
