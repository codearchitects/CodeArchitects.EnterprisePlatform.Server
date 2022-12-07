using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal record OTMAssociation(
  AssociationKind Kind,
  Type From,
  Type To,
  MemberInfo? Navigation,
  MemberInfo? InverseNavigation,
  IReadOnlyCollection<Name> ForeignKeyNames)
  : SimpleAssociation(Kind, From, To, Navigation, InverseNavigation, ForeignKeyNames)
{
  public override AssociationMultiplicity Multiplicity => AssociationMultiplicity.OneToMany;
}
