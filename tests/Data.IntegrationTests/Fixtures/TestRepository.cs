using CodeArchitects.Platform.Data.Navigation;

namespace CodeArchitects.Platform.Data.Fixtures;

internal class TestRepository<TEntity, TKey> : IRepository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private readonly IRepository<TEntity, TKey> _implementation;
  private readonly bool _async;

  public TestRepository(IRepository<TEntity, TKey> implementation, bool async)
  {
    _implementation = implementation;
    _async = async;
  }

  public Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    if (_async)
      return _implementation.FindAsync(key, cancellationToken);

    return Task.FromResult(_implementation.Find(key));
  }

  public Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    if (_async)
      return _implementation.FindAsync(key, includeAction, cancellationToken);

    return Task.FromResult(_implementation.Find(key, includeAction));
  }

  public Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    if (_async)
      return _implementation.InsertAsync(entity, cancellationToken);

    _implementation.Insert(entity);
    return Task.CompletedTask;
  }

  public Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
  {
    if (_async)
      return _implementation.InsertManyAsync(entities, cancellationToken);

    _implementation.InsertMany(entities);
    return Task.CompletedTask;
  }

  public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    if (_async)
      return _implementation.UpdateAsync(entity, cancellationToken);

    _implementation.Update(entity);
    return Task.CompletedTask;
  }

  public Task UpdateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
  {
    if (_async)
      return _implementation.UpdateManyAsync(entities, cancellationToken);

    _implementation.UpdateMany(entities);
    return Task.CompletedTask;
  }

  public Task UpsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    if (_async)
      return _implementation.UpsertAsync(entity, cancellationToken);

    _implementation.Upsert(entity);
    return Task.CompletedTask;
  }

  public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    if (_async)
      return _implementation.RemoveAsync(entity, cancellationToken);

    _implementation.Remove(entity);
    return Task.CompletedTask;
  }

  public Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    if (_async)
      return _implementation.RemoveAsync(key, cancellationToken);

    _implementation.Remove(key);
    return Task.CompletedTask;
  }

  #region Sync methods

  public TEntity? Find(TKey key) => throw UseAsyncMethod();
  public TEntity? Find(TKey key, IncludeAction<TEntity> includeAction) => throw UseAsyncMethod();
  public void Insert(TEntity entity) => throw UseAsyncMethod();
  public void InsertMany(IEnumerable<TEntity> entities) => throw UseAsyncMethod();
  public void Update(TEntity entity) => throw UseAsyncMethod();
  public void UpdateMany(IEnumerable<TEntity> entities) => throw UseAsyncMethod();
  public void Upsert(TEntity entity) => throw UseAsyncMethod();
  public void Remove(TEntity entity) => throw UseAsyncMethod();
  public void Remove(TKey key) => throw UseAsyncMethod();

  private static Exception UseAsyncMethod() => new NotImplementedException("Use async methods in tests. Sync and async implementations will be switched dynamically");

  #endregion
}
