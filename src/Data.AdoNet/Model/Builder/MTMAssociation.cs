using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal record MTMAssociation(
  Type From,
  Type To,
  MemberInfo? Navigation,
  MemberInfo? InverseNavigation,
  string TableName,
  IReadOnlyCollection<string> ForeignKeyNames)
  : Association(AssociationKind.Composition, From, To, Navigation, InverseNavigation)
{
  public override AssociationMultiplicity Multiplicity => AssociationMultiplicity.ManyToMany;
}
