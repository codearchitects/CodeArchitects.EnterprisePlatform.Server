using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.GraphQL.Fixtures;

internal class NodeEqualityComparer :
  IEqualityComparer<IOperationDefinitionNode>,
  IEqualityComparer<IDirectiveNode>,
  IEqualityComparer<ISelectionSetNode>,
  IEqualityComparer<ISelectionNode>,
  IEqualityComparer<IArgumentNode>,
  IEqualityComparer<IListValueNode>,
  IEqualityComparer<IObjectValueNode>,
  IEqualityComparer<IObjectFieldNode>,
  IEqualityComparer<object?>
{
  public static readonly NodeEqualityComparer Instance = new();

  public bool Equals(IOperationDefinitionNode? x, IOperationDefinitionNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.OperationType != y.OperationType)
      return false;

    if (x.Name != y.Name)
      return false;

    if (!x.Variables.SequenceEqual(y.Variables, ModelEqualityComparer.Instance))
      return false;

    if (!x.Directives.SequenceEqual(y.Directives, this))
      return false;

    if (!Equals(x.SelectionSet, y.SelectionSet))
      return false;

    return true;
  }

  public bool Equals(IDirectiveNode? x, IDirectiveNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.Name != y.Name)
      return false;

    if (!x.Arguments.SequenceEqual(y.Arguments, this))
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

  public bool Equals(IArgumentNode? x, IArgumentNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.Name != y.Name)
      return false;

    if (!Equals(x.Value, y.Value))
      return false;

    return true;
  }

  public bool Equals(ISelectionNode? x, ISelectionNode? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.Alias != y.Alias)
      return false;

    if (x.FieldName != y.FieldName)
      return false;

    if (!x.Arguments.SequenceEqual(y.Arguments, this))
      return false;

    if (!x.Directives.SequenceEqual(y.Directives, this))
      return false;

    if (!Equals(x.SelectionSet, y.SelectionSet))
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

    if (x.Name != y.Name)
      return false;

    if (!Equals(x.Value, y.Value))
      return false;

    return true;
  }

  public new bool Equals(object? x, object? y)
  {
    if (x is IObjectValueNode xObjectValue)
      return Equals(xObjectValue, y as IObjectValueNode);

    if (x is IListValueNode xListValue)
      return Equals(xListValue, y as IListValueNode);

    return object.Equals(x, y);
  }

  public int GetHashCode([DisallowNull] IOperationDefinitionNode obj) => 0;
  public int GetHashCode([DisallowNull] IDirectiveNode obj) => 0;
  public int GetHashCode([DisallowNull] ISelectionSetNode obj) => 0;
  public int GetHashCode([DisallowNull] IArgumentNode obj) => 0;
  public int GetHashCode([DisallowNull] ISelectionNode obj) => 0;
  public int GetHashCode([DisallowNull] IObjectValueNode obj) => 0;
  public int GetHashCode([DisallowNull] IObjectFieldNode obj) => 0;
  public int GetHashCode([DisallowNull] object? obj) => 0;
  public int GetHashCode([DisallowNull] IListValueNode obj) => 0;
}
