namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal record ManyToManyAssociation(
  Type From,
  Type To,
  string NavigationName,
  string? InverseNavigationName)
  : Association(From, To, NavigationName, InverseNavigationName)
{
  public override AssociationKind Kind => AssociationKind.ManyToMany;
}
