using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Materialization;

internal interface IDefaultEntityFactoryCache
{
  void AddFactory<TEntity, TKey>(Func<TKey, TEntity> factory)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  bool TryGetFactory<TEntity, TKey>([NotNullWhen(true)] out Func<TKey, TEntity>? factory)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
