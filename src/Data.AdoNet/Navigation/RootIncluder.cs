using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class RootIncluder<TEntity, TKey> : Includer<TEntity>, IIncluder<TEntity>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public RootIncluder(IEntityModel<TEntity, TKey> entity)
  {
    Root = new NavigationRoot<TEntity, TKey>(entity);
  }

  public NavigationRoot<TEntity, TKey> Root { get; }

  protected override NavigationNode Node => Root;

  public IStringIncluder<TEntity> Include(string navigation)
  {
    Root.AddLeaf(navigation);

    return this;
  }
}
