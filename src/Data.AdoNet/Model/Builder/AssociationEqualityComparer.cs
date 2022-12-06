using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class AssociationEqualityComparer : EqualityComparer<Association>
{
  public static readonly AssociationEqualityComparer Instance = new();

  private AssociationEqualityComparer() { }

  public override bool Equals([AllowNull] Association x, [AllowNull] Association y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.From != y.From)
      return false;

    if (x.To != y.To)
      return false;

    return x.Navigation == y.Navigation || x.InverseNavigation == y.InverseNavigation;
  }

  public override int GetHashCode([DisallowNull] Association obj)
  {
    return HashCode.Combine(obj.From, obj.To);
  }
}
