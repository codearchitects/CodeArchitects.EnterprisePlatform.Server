using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationRoot<TEntity, TKey> : NavigationNode, INavigationRoot<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public NavigationRoot(IEntityModel<TEntity, TKey> entity)
  {
    Entity = entity;
  }

  public override IEntityModel Target => Entity;

  public IEntityModel<TEntity, TKey> Entity { get; }

  public IReadOnlyCollection<INavigation> Navigations => Children;

  IEntityModel INavigationRoot.Entity => Entity;

  public override bool Equals(object? obj)
  {
    if (obj is not NavigationRoot<TEntity, TKey> other)
      return false;

    return
      Entity == other.Entity &&
      NavigationCollectionEqualityComparer.Instance.Equals(Navigations, other.Navigations);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(
      Entity,
      NavigationCollectionEqualityComparer.Instance.GetHashCode(Navigations));
  }
}
