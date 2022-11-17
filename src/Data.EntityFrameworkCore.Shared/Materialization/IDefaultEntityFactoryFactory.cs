using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Materialization;

internal interface IDefaultEntityFactoryFactory
{
  bool TryCreateFactory<TEntity, TKey>([NotNullWhen(true)] out Func<TKey, TEntity>? factory)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
