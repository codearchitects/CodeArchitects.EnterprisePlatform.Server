using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal readonly record struct NavigationSpec(IEntityModel Entity, IReadOnlyCollection<INavigation> Navigations)
{
  public bool Equals(NavigationSpec other)
  {
    return
      other.Entity.Equals(Entity) &&
      NavigationCollection.Equal(other.Navigations, Navigations);
  }

  public override int GetHashCode()
  {
    return Entity.GetHashCode() + Navigations.Count;
  }

  public static NavigationSpec FromEntity(IEntityModel entity)
    => new NavigationSpec(entity, Array.Empty<INavigation>());

  public static NavigationSpec FromNavigation(NavigationRoot root)
    => new NavigationSpec(root.Target, root.Children);
}
