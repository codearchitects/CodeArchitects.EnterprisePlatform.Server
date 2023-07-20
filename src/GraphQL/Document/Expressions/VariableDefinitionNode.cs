using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class VariableDefinitionNode : IVariableDefinitionNode, IVariableNode, INamedTypeNode, IListTypeNode, INonNullTypeNode
{
  private readonly IVariable _variable;
  private IType _currentType;
  private bool _consumedNonNullable;

  public VariableDefinitionNode(IVariable variable)
  {
    _variable = variable;
    _currentType = variable.Type;
  }

  public ValueNodeKind ValueKind => ValueNodeKind.Variable;

  public IVariableNode Variable => this;

  public ITypeNode Type => this;

  ReadOnlySpan<char> IVariableNode.Name => _variable.Name;

  TypeNodeKind ITypeNode.TypeKind
  {
    get
    {
      if (!_consumedNonNullable && !_currentType.IsNullable)
        return TypeNodeKind.NonNullType;

      _consumedNonNullable = false;

      return _currentType.Kind is TypeKind.List ? TypeNodeKind.ListType : TypeNodeKind.NamedType;
    }
  }

  ReadOnlySpan<char> INamedTypeNode.Name
  {
    get
    {
      Debug.Assert(_currentType.Kind is not TypeKind.List);

      return ((INamedType)_currentType).Name;
    }
  }

  ITypeNode IListTypeNode.ItemType
  {
    get
    {
      Debug.Assert(_currentType.Kind is TypeKind.List);

      _currentType = ((IListType)_currentType).ItemType;
      return this;
    }
  }

  ITypeNode INonNullTypeNode.NullableType
  {
    get
    {
      _consumedNonNullable = true;
      return this;
    }
  }
}
