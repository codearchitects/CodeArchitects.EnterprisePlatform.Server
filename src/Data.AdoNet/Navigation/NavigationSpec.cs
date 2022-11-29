using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal readonly record struct NavigationSpec(IEntityModel Entity, IReadOnlyCollection<INavigation> Navigations)
{
  public NavigationSpec(IEntityModel entity)
    : this(entity, Array.Empty<INavigation>())
  {
  }

  public NavigationSpec(NavigationRoot root)
    : this(root.Target, root.Children)
  {
  }

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
}
