using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IListTypeNode
{
  ITypeNode IListTypeNode.ItemType => this;
}