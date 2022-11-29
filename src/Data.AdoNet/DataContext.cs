using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Data;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal class DataContext<TDbConnection> : IDataContext<TDbConnection>
  where TDbConnection : DbConnection
{
  private readonly IStateManager<TDbConnection> _stateManager;
  private readonly IExecutor _executor;
  private readonly IPersistenceModel _model;

  public DataContext(IStateManager<TDbConnection> stateManager, IExecutor executor, IPersistenceModel model)
  {
    _stateManager = stateManager;
    _executor = executor;
    _model = model;
  }

  public TDbConnection Connection => _stateManager.Connection;

  IDbConnection IDataContext.Connection => _stateManager.Connection;

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel entityModel = EnsureEntity<TEntity>();
    EnsureKey<TKey>(entityModel);

    return FindAsyncCore<TEntity, TKey>(key, new NavigationSpec(entityModel), cancellationToken);
  }

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel entityModel = EnsureEntity<TEntity>();
    EnsureKey<TKey>(entityModel);

    Includer<TEntity> includer = new(entityModel);
    includeAction(includer);

    return FindAsyncCore<TEntity, TKey>(key, includer.Spec, cancellationToken);
  }

  public Task InsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    throw new NotImplementedException();
  }

  public Task UpdateAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    throw new NotImplementedException();
  }

  public Task RemoveAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    throw new NotImplementedException();
  }

  public Task RemoveAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    throw new NotImplementedException();
  }

  public Task BatchExecuteAsync(Execution<TDbConnection> execution, CancellationToken cancellationToken = default)
  {
    return _stateManager.ExecuteAsync(execution, cancellationToken);
  }

  public Task BatchExecuteAsync(Execution<IDbConnection> execution, CancellationToken cancellationToken = default)
  {
    return _stateManager.ExecuteAsync(execution, cancellationToken);
  }

  private async Task<TEntity?> FindAsyncCore<TEntity, TKey>(TKey key, NavigationSpec spec, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    using DbCommand command = Connection.CreateCommand();

    await Connection.OpenAsync(cancellationToken);
    try
    {
      return await _executor.ExecuteSelectCommandAsync<TEntity, TKey>(command, key, spec, cancellationToken);
    }
    finally
    {
      Connection.Close();
    }
  }

  private IEntityModel EnsureEntity<TEntity>()
  {
    if (!_model.TryGetEntity(typeof(TEntity), out IEntityModel entityModel))
      throw new InvalidOperationException($"'{typeof(TEntity).Name}' is not registered as a database entity.");

    return entityModel;
  }

  private void EnsureKey<TKey>(IEntityModel entityModel)
  {
    if (entityModel.PrimaryKey.Type != typeof(TKey))
      throw new InvalidOperationException($"Expected key of type '{entityModel.PrimaryKey.Type.Name}' for entity of type '{entityModel.Type.Name}', but '{typeof(TKey).Name}' was provided.");
  }
}
