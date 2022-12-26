using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;
using Microsoft.Extensions.Caching.Memory;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationTreeFactory : INavigationTreeFactory
{
  private readonly IMemoryCache _cache;

  public NavigationTreeFactory(IMemoryCache cache)
  {
    _cache = cache;
  }

  public INavigationRoot<TEntity, TKey> Create<TEntity, TKey>(IEntityModel<TEntity, TKey> entityModel, IncludeAction<TEntity> include)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (_cache.TryGetValue(include, out INavigationRoot<TEntity, TKey> root))
      return root;

    lock (include)
    {
      if (_cache.TryGetValue(include, out root))
        return root;

      RootIncluder<TEntity, TKey> includer = new(entityModel);
      include(includer);
      root = includer.Root;

      _cache.Set(include, root);
    }

    return root;
  }

  public INavigationRoot<TEntity, TKey> CreateEmpty<TEntity, TKey>(IEntityModel<TEntity, TKey> entityModel)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return new EmptyNavigationRoot<TEntity, TKey>(entityModel);
  }
}
