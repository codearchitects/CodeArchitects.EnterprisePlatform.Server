using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class TypeConditionNode : ITypeConditionNode, INamedTypeNode
{
  private readonly INamedType _type;

  public TypeConditionNode(INamedType type)
  {
    _type = type;
  }

  public INamedTypeNode Type => this;

  public TypeNodeKind TypeKind => TypeNodeKind.NamedType;

  public ReadOnlySpan<char> Name => _type.Name;
}
