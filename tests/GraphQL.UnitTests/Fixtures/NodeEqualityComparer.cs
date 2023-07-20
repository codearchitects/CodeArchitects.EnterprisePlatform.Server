using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.UnitTests.FluentMock;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.GraphQL.Fixtures;

internal class NodeEqualityComparer :
  IEqualityComparer<IDocumentNode>,
  IEqualityComparer<IDefinitionNode>,
  IEqualityComparer<IOperationDefinitionNode>,
  IEqualityComparer<IFragmentDefinitionNode>,
  IEqualityComparer<IVariableDefinitionListNode>,
  IEqualityComparer<IVariableDefinitionNode>,
  IEqualityComparer<IVariableNode>,
  IEqualityComparer<IDirectiveListNode>,
  IEqualityComparer<IDirectiveNode>,
  IEqualityComparer<ISelectionSetNode>,
  IEqualityComparer<ISelectionNode>,
  IEqualityComparer<IFieldNode>,
  IEqualityComparer<IFragmentSpreadNode>,
  IEqualityComparer<IInlineFragmentNode>,
  IEqualityComparer<ITypeConditionNode>,
  IEqualityComparer<ITypeNode>,
  IEqualityComparer<INamedTypeNode>,
  IEqualityComparer<IListTypeNode>,
  IEqualityComparer<INonNullTypeNode>,
  IEqualityComparer<IArgumentListNode>,
  IEqualityComparer<IArgumentNode>,
  IEqualityComparer<IListValueNode>,
  IEqualityComparer<IObjectValueNode>,
  IEqualityComparer<IObjectFieldNode>,
  IEqualityComparer<object?>
{
  public static readonly NodeEqualityComparer Instance = new();

  public bool Equals(IDocumentNode? x, IDocumentNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Definitions.SequenceEqual(y.Definitions, this))
      return false;

    return true;
  }

  public bool Equals(IDefinitionNode? x, IDefinitionNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return y switch
    {
      IOperationDefinitionNode yOperationDefinition => Equals(x as IOperationDefinitionNode, yOperationDefinition),
      IFragmentDefinitionNode yFragmentDefinition   => Equals(x as IFragmentDefinitionNode, yFragmentDefinition),
      _                                             => throw Errors.Unreachable
    };
  }

  public bool Equals(IOperationDefinitionNode? x, IOperationDefinitionNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!EqualsCore(x, y))
      return false;

    if (x.IsQueryShortHand != y.IsQueryShortHand)
      return false;

    if (x.OperationType != y.OperationType)
      return false;

    if (!x.Name.SequenceEqual(y.Name))
      return false;

    if (!Equals(x.VariableDefinitionList, y.VariableDefinitionList))
      return false;

    if (!Equals(x.DirectiveList, y.DirectiveList))
      return false;

    if (!Equals(x.SelectionSet, y.SelectionSet))
      return false;

    return true;
  }

  public bool Equals(IFragmentDefinitionNode? x, IFragmentDefinitionNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!EqualsCore(x, y))
      return false;

    if (!x.FragmentName.SequenceEqual(y.FragmentName))
      return false;

    if (!Equals(x.TypeCondition, y.TypeCondition))
      return false;

    if (!Equals(x.DirectiveList, y.DirectiveList))
      return false;

    if (!Equals(x.SelectionSet, y.SelectionSet))
      return false;

    return true;
  }

  private static bool EqualsCore(IDefinitionNode x, IDefinitionNode y)
  {
    if (!MockHelper.IsSetUp(y, node => node.DefinitionKind))
      return true;

    return x.DefinitionKind == y.DefinitionKind;
  }

  public bool Equals(IVariableDefinitionListNode? x, IVariableDefinitionListNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.VariableDefinitions.SequenceEqual(y.VariableDefinitions, this))
      return false;

    return true;
  }

  public bool Equals(IVariableDefinitionNode? x, IVariableDefinitionNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!Equals(x.Variable, y.Variable))
      return false;

    if (!Equals(x.Type, y.Type))
      return false;

    return true;
  }

  public bool Equals(IVariableNode? x, IVariableNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Name.SequenceEqual(y.Name))
      return false;

    return true;
  }

  public bool Equals(IDirectiveListNode? x, IDirectiveListNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Directives.SequenceEqual(y.Directives, this))
      return false;

    return true;
  }

  public bool Equals(IDirectiveNode? x, IDirectiveNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Name.SequenceEqual(y.Name))
      return false;

    if (!Equals(x.ArgumentList, y.ArgumentList))
      return false;

    return true;
  }

  public bool Equals(ISelectionSetNode? x, ISelectionSetNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Selections.SequenceEqual(y.Selections, this))
      return false;

    return true;
  }

  public bool Equals(ISelectionNode? x, ISelectionNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return y switch
    {
      IFieldNode yField                    => Equals(x as IFieldNode, yField),
      IFragmentSpreadNode yFragmentSpread  => Equals(x as IFragmentSpreadNode, yFragmentSpread),
      IInlineFragmentNode yInlineFragment  => Equals(x as IInlineFragmentNode, yInlineFragment),
      _                                    => throw Errors.Unreachable,
    };
  }

  public bool Equals(IFieldNode? x, IFieldNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!EqualsCore(x, y))
      return false;

    if (!x.Alias.SequenceEqual(y.Alias))
      return false;

    if (!x.FieldName.SequenceEqual(y.FieldName))
      return false;

    if (!Equals(x.ArgumentList, y.ArgumentList))
      return false;

    if (!Equals(x.DirectiveList, y.DirectiveList))
      return false;

    if (!Equals(x.SelectionSet, y.SelectionSet))
      return false;

    return true;
  }

  public bool Equals(IFragmentSpreadNode? x, IFragmentSpreadNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!EqualsCore(x, y))
      return false;

    if (!x.FragmentName.SequenceEqual(y.FragmentName))
      return false;

    if (!Equals(x.DirectiveList, y.DirectiveList))
      return false;

    return true;
  }

  public bool Equals(IInlineFragmentNode? x, IInlineFragmentNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!EqualsCore(x, y))
      return false;

    if (!Equals(x.TypeCondition, y.TypeCondition))
      return false;

    if (!Equals(x.DirectiveList, y.DirectiveList))
      return false;

    if (!Equals(x.SelectionSet, y.SelectionSet))
      return false;

    return true;
  }

  private static bool EqualsCore(ISelectionNode x, ISelectionNode y)
  {
    if (!MockHelper.IsSetUp(y, node => node.SelectionKind))
      return true;

    return x.SelectionKind == y.SelectionKind;
  }

  public bool Equals(ITypeConditionNode? x, ITypeConditionNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!Equals(x.Type, y.Type))
      return false;

    return true;
  }

  public bool Equals(ITypeNode? x, ITypeNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return y switch
    {
      INamedTypeNode yNamed     => Equals(x as INamedTypeNode, yNamed),
      IListTypeNode yList       => Equals(x as IListTypeNode, yList),
      INonNullTypeNode yNonNull => Equals(x as INonNullTypeNode, yNonNull),
      _                         => throw Errors.Unreachable,
    };
  }

  public bool Equals(INamedTypeNode? x, INamedTypeNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!EqualsCore(x, y))
      return false;

    if (!x.Name.SequenceEqual(y.Name))
      return false;

    return true;
  }

  public bool Equals(IListTypeNode? x, IListTypeNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!EqualsCore(x, y))
      return false;

    if (!Equals(x.ItemType, y.ItemType))
      return false;

    return true;
  }

  public bool Equals(INonNullTypeNode? x, INonNullTypeNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!EqualsCore(x, y))
      return false;

    if (!Equals(x.NullableType, y.NullableType))
      return false;

    return true;
  }

  private static bool EqualsCore(ITypeNode x, ITypeNode y)
  {
    if (!MockHelper.IsSetUp(y, node => node.TypeKind))
      return true;

    return x.TypeKind == y.TypeKind;
  }

  public bool Equals(IArgumentListNode? x, IArgumentListNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Arguments.SequenceEqual(y.Arguments, this))
      return false;

    return true;
  }

  public bool Equals(IArgumentNode? x, IArgumentNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Name.SequenceEqual(y.Name))
      return false;

    if (!Equals(x.Value, y.Value))
      return false;

    return true;
  }

  public bool Equals(IListValueNode? x, IListValueNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Values.SequenceEqual(y.Values, this))
      return false;

    return true;
  }

  public bool Equals(IObjectValueNode? x, IObjectValueNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Fields.SequenceEqual(y.Fields, this))
      return false;

    return true;
  }

  public bool Equals(IObjectFieldNode? x, IObjectFieldNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.Name.SequenceEqual(y.Name))
      return false;

    if (!Equals(x.Value, y.Value))
      return false;

    return true;
  }

  public new bool Equals(object? x, object? y)
  {
    if (y is IObjectValueNode yObjectValue)
      return Equals(x as IObjectValueNode, yObjectValue);

    if (y is IListValueNode yListValue)
      return Equals(x as IListValueNode, yListValue);

    if (y is IVariableNode yVariable)
      return Equals(x as IVariableNode, yVariable);

    return object.Equals(x, y);
  }

  public int GetHashCode([DisallowNull] IOperationDefinitionNode obj) => 0;
  public int GetHashCode([DisallowNull] IVariableDefinitionListNode obj) => 0;
  public int GetHashCode([DisallowNull] IVariableDefinitionNode obj) => 0;
  public int GetHashCode([DisallowNull] IVariableNode obj) => 0;
  public int GetHashCode([DisallowNull] IDirectiveListNode obj) => 0;
  public int GetHashCode([DisallowNull] IDirectiveNode obj) => 0;
  public int GetHashCode([DisallowNull] ISelectionSetNode obj) => 0;
  public int GetHashCode([DisallowNull] ISelectionNode obj) => 0;
  public int GetHashCode([DisallowNull] IFieldNode obj) => 0;
  public int GetHashCode([DisallowNull] IFragmentSpreadNode obj) => 0;
  public int GetHashCode([DisallowNull] IInlineFragmentNode obj) => 0;
  public int GetHashCode([DisallowNull] ITypeNode obj) => 0;
  public int GetHashCode([DisallowNull] INamedTypeNode obj) => 0;
  public int GetHashCode([DisallowNull] IListTypeNode obj) => 0;
  public int GetHashCode([DisallowNull] INonNullTypeNode obj) => 0;
  public int GetHashCode([DisallowNull] IArgumentListNode obj) => 0;
  public int GetHashCode([DisallowNull] IArgumentNode obj) => 0;
  public int GetHashCode([DisallowNull] IListValueNode obj) => 0;
  public int GetHashCode([DisallowNull] IObjectValueNode obj) => 0;
  public int GetHashCode([DisallowNull] IObjectFieldNode obj) => 0;
  public int GetHashCode([DisallowNull] object? obj) => 0;
  public int GetHashCode([DisallowNull] ITypeConditionNode obj) => 0;
  public int GetHashCode([DisallowNull] IDocumentNode obj) => 0;
  public int GetHashCode([DisallowNull] IFragmentDefinitionNode obj) => 0;
  public int GetHashCode([DisallowNull] IDefinitionNode obj) => 0;
}
