using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class EmptyNavigationRoot<TEntity, TKey> : INavigationRoot<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public EmptyNavigationRoot(IEntityModel<TEntity, TKey> entity)
  {
    Entity = entity;
  }

  public IEntityModel<TEntity, TKey> Entity { get; }

  public IReadOnlyCollection<INavigation> Navigations => Array.Empty<INavigation>();

  IEntityModel INavigationRoot.Entity => Entity;

  public override bool Equals(object? obj)
  {
    if (obj is not EmptyNavigationRoot<TEntity, TKey> other)
      return false;

    return Entity == other.Entity;
  }

  public override int GetHashCode()
  {
    return Entity.GetHashCode();
  }
}
