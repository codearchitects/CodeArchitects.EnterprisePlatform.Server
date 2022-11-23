using CodeArchitects.Platform.Data.Navigation;

namespace CodeArchitects.Platform.Data;

public delegate void IncludeAction<TEntity>(IIncluder<TEntity> includer)
  where TEntity : class;

public interface IRepository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default);
  Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default);
  Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
  Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
  Task RemoveAsync(TKey key, CancellationToken cancellationToken = default);
}

public static class C
{
  public static Index I() => default;
}