namespace CodeArchitects.Platform.Data;

public interface IDataContext
{
  Task<TEntity?> FindAsync<TEntity, TKey>(string entityName, TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task<TEntity?> FindAsync<TEntity, TKey>(string entityName, TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task InsertAsync<TEntity, TKey>(string entityName, TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task UpdateAsync<TEntity, TKey>(string entityName, TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task RemoveAsync<TEntity, TKey>(string entityName, TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task RemoveAsync<TEntity, TKey>(string entityName, TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
