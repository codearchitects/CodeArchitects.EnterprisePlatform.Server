using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal abstract record SimpleAssociation(
  AssociationKind Kind,
  Type From,
  Type To,
  MemberInfo? Navigation,
  MemberInfo? InverseNavigation,
  IReadOnlyCollection<Name> ForeignKeyNames)
  : Association(Kind, From, To, Navigation, InverseNavigation)
{
}
