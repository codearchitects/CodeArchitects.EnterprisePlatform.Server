using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Materialization;

internal class DefaultEntityFactory : IDefaultEntityFactory
{
  private readonly IDefaultEntityFactoryFactory _factory;
  private readonly IDefaultEntityFactoryCache _cache;

  public DefaultEntityFactory(IDefaultEntityFactoryFactory factory, IDefaultEntityFactoryCache cache)
  {
    _factory = factory;
    _cache = cache;
  }

  public bool TryCreate<TEntity, TKey>(TKey key, [NotNullWhen(true)] out TEntity? entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (!_cache.TryGetFactory(out Func<TKey, TEntity>? factory))
    {
      if (!_factory.TryCreateFactory(out factory))
      {
        entity = null;
        return false;
      }

      _cache.AddFactory(factory);
    }

    entity = factory(key);
    return true;
  }
}
