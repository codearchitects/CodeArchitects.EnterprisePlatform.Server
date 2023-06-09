using CodeArchitects.Platform.GraphQL.Model;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.GraphQL.Fixtures;

internal class ModelEqualityComparer :
  IEqualityComparer<IType>,
  IEqualityComparer<IVariable>,
  IEqualityComparer<IField>
{
  public static readonly ModelEqualityComparer Instance = new();

  public bool Equals(IVariable? x, IVariable? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return
      x.Name == y.Name &&
      Equals(x.Type, y.Type);
  }

  public bool Equals(IType? x, IType? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return x switch
    {
      IScalarType scalarType => Equals(scalarType, y as IScalarType),
      IObjectType objectType => Equals(objectType, y as IObjectType),
      _                      => throw new NotSupportedException()
    };
  }

  public bool Equals(IField? x, IField? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return
      x.Name == y.Name &&
      Equals(x.Type, y.Type);
  }

  private bool Equals(IObjectType? x, IObjectType? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return
      EqualsCore(x, y) &&
      x.Fields.SequenceEqual(y.Fields, this);
  }

  private static bool Equals(IScalarType? x, IScalarType? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return EqualsCore(x, y);
  }

  private static bool EqualsCore(IType x, IType y)
  {
    return
      x.Name == y.Name &&
      x.ClrType == y.ClrType &&
      x.Kind == y.Kind;
  }

  public int GetHashCode([DisallowNull] IVariable obj) => 0;
  public int GetHashCode([DisallowNull] IType obj) => 0;
  public int GetHashCode([DisallowNull] IField obj) => 0;
}
