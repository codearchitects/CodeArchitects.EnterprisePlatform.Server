using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Materialization;

internal interface IDefaultEntityFactory
{
  bool TryCreate<TEntity, TKey>(TKey key, [NotNullWhen(true)] out TEntity? entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
