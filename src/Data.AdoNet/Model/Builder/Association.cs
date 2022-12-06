using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal abstract record Association(
  AssociationKind Kind,
  Type From,
  Type To,
  MemberInfo? Navigation,
  MemberInfo? InverseNavigation)
{
  public abstract AssociationMultiplicity Multiplicity { get; }
}
