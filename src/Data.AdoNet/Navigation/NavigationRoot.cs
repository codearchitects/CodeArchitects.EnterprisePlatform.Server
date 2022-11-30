using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationRoot<TEntity, TKey> : IncluderNode
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public NavigationRoot(IEntityModel<TEntity, TKey> entity)
  {
    Entity = entity;
  }

  public override IEntityModel Target => Entity;

  public IEntityModel<TEntity, TKey> Entity { get; }
}
