namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal record OneToManyAssociation(
  Type From,
  Type To,
  string NavigationName,
  string? InverseNavigationName,
  IReadOnlyCollection<string> ForeignKeyNames)
  : Association(From, To, NavigationName, InverseNavigationName)
{
  public override AssociationKind Kind => AssociationKind.OneToMany;
}
