using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

public interface IDataContext
{
  IDbConnection Connection { get; }

  Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task InsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task UpdateAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task RemoveAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task RemoveAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task BatchExecuteAsync(Execution<IDbConnection> execution, CancellationToken cancellationToken = default);
}
