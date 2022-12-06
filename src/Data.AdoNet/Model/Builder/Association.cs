namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal abstract record Association(
  Type From,
  Type To,
  string NavigationName,
  string? InverseNavigationName)
{
  public abstract AssociationKind Kind { get; }
}
