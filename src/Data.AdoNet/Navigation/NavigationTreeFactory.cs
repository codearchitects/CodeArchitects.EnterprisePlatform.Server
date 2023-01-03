using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationTreeFactory : INavigationTreeFactory
{
  private readonly IMemoryCache _cache;
  private readonly ConcurrentDictionary<object, object> _locks = new();

  public NavigationTreeFactory(IMemoryCache cache)
  {
    _cache = cache;
    _locks = new();
  }

  public INavigationRoot<TEntity, TKey> Create<TEntity, TKey>(IEntityModel<TEntity, TKey> entityModel, IncludeAction<TEntity> include)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (_cache.TryGetValue(include, out INavigationRoot<TEntity, TKey> root))
      return root;

    object navigationLock = _locks.GetOrAdd(entityModel, _ => new object());
    try
    {
      lock (navigationLock)
      {
        if (_cache.TryGetValue(include, out root))
          return root;

        RootIncluder<TEntity, TKey> includer = new(entityModel);
        include(includer);
        root = includer.Root;

        ICacheEntry entry = _cache.CreateEntry(include);
        entry.Value = root;
        entry.Size = ComputeEntrySize(root);
      }
    }
    finally
    {
      _locks.Remove(navigationLock, out _);
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
