using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal record OTOAssociation(
  AssociationKind Kind,
  Type From,
  Type To,
  MemberInfo? Navigation,
  MemberInfo? InverseNavigation,
  IReadOnlyCollection<Name> ForeignKeyNames)
  : Association(Kind, From, To, Navigation, InverseNavigation)
{
  public override AssociationMultiplicity Multiplicity => AssociationMultiplicity.OneToOne;
}
