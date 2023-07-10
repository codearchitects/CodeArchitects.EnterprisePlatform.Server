using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;
using Microsoft.Extensions.Caching.Memory;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationTreeFactory : INavigationTreeFactory
{
  private readonly Synchronizer _synchronizer;
  private readonly IMemoryCache _cache;

  public NavigationTreeFactory(Synchronizer synchronizer, IMemoryCache cache)
  {
    _synchronizer = synchronizer;
    _cache = cache;
  }

  public INavigationRoot<TEntity, TKey> Create<TEntity, TKey>(IEntityModel<TEntity, TKey> entityModel, IncludeAction<TEntity> include)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (_cache.TryGetValue(include, out INavigationRoot<TEntity, TKey> root))
      return root;

    using (_synchronizer.Sync(include))
    {
      if (_cache.TryGetValue(include, out root))
        return root;

      RootIncluder<TEntity, TKey> includer = new(entityModel);
      include(includer);
      root = includer.Root;

      using ICacheEntry entry = _cache.CreateEntry(include);
      entry.Value = root;
      entry.Size = ComputeEntrySize(root);
    }

    return root;
  }

  public INavigationRoot<TEntity, TKey> CreateEmpty<TEntity, TKey>(IEntityModel<TEntity, TKey> entityModel)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return new EmptyNavigationRoot<TEntity, TKey>(entityModel);
  }

  private static long ComputeEntrySize(INavigationRoot root)
  {
    long size = 2;
    foreach (INavigation navigation in root.Navigations)
    {
      size += ComputeSize(navigation);
    }

    return size;

    static long ComputeSize(INavigation navigation)
    {
      long size = 2;
      foreach (INavigation child in navigation.Children)
      {
        size += ComputeSize(child);
      }

      return size;
    }
  }
}
