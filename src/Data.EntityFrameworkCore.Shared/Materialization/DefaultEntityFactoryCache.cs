using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Materialization;

internal class DefaultEntityFactoryCache : IDefaultEntityFactoryCache
{
  private readonly IDictionary<Type, Delegate> _factories;

  public DefaultEntityFactoryCache(IDictionary<Type, Delegate> factories)
  {
    _factories = factories;
  }

  public void AddFactory<TEntity, TKey>(Func<TKey, TEntity> factory)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _factories.Add(typeof(TEntity), factory);
  }

  public bool TryGetFactory<TEntity, TKey>([NotNullWhen(true)] out Func<TKey, TEntity>? factory)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (!_factories.TryGetValue(typeof(TEntity), out Delegate? @delegate))
    {
      factory = null;
      return false;
    }

    factory = (Func<TKey, TEntity>)@delegate;
    return true;
  }

  public static DefaultEntityFactoryCache Create()
  {
    return new(new ConcurrentDictionary<Type, Delegate>());
  }
}
