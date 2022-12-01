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

  public static NavigationSpec<TEntity, TKey> FromEntity<TEntity, TKey>(IEntityModel<TEntity, TKey> entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
    => new(entity, Array.Empty<INavigation>());

  public static NavigationSpec<TEntity, TKey> FromNavigation<TEntity, TKey>(NavigationRoot<TEntity, TKey> root)
    where TEntity : class
    where TKey : IEquatable<TKey>
    => new(root.Entity, root.Children);
}
