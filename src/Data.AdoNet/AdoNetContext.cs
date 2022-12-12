using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Data;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal class AdoNetContext<TDbConnection> : IAdoNetContext<TDbConnection>
  where TDbConnection : DbConnection
{
  private readonly IStateManager<TDbConnection> _stateManager;
  private readonly IExecutor _executor;
  private readonly IDataModel _model;

  public AdoNetContext(IStateManager<TDbConnection> stateManager, IExecutor executor, IDataModel model)
  {
    _stateManager = stateManager;
    _executor = executor;
    _model = model;
  }

  public TDbConnection Connection => _stateManager.Connection;

  IDbConnection IAdoNetContext.Connection => _stateManager.Connection;

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> entityModel = EnsureEntity<TEntity, TKey>();

    return FindAsyncCore(key, NavigationSpec.FromEntity(entityModel), cancellationToken);
  }

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> entityModel = EnsureEntity<TEntity, TKey>();

    RootIncluder<TEntity, TKey> includer = new(entityModel);
    includeAction(includer);

    return FindAsyncCore(key, includer.Spec, cancellationToken);
  }

  public Task InsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    return _stateManager.ExecuteAsync(async (connection, transaction, cancellationToken) =>
    {
      using DbCommand command = connection.CreateCommand();
      command.Transaction = transaction;
      await _executor.ExecuteInsertCommandAsync(command, entity, model, cancellationToken);
    }, cancellationToken);
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

  public Task BatchExecuteAsync(Execution<TDbConnection, DbTransaction> execution, CancellationToken cancellationToken = default)
  {
    return _stateManager.ExecuteAsync(execution, cancellationToken);
  }

  public Task BatchExecuteAsync(Execution<IDbConnection, IDbTransaction> execution, CancellationToken cancellationToken = default)
  {
    return _stateManager.ExecuteAsync(execution, cancellationToken);
  }

  public void VisitGraph<TEntity, TState>(TEntity entity, TState state, VisitNodeCallback<TState> callback)
    where TEntity : class
  {
    IEntityModel model = EnsureEntity<TEntity>();

    callback(entity, model, default, state);
    _executor.VisitGraph(entity, model, state, callback);
  }

  public async Task VisitGraphAsync<TEntity, TState>(TEntity entity, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken)
    where TEntity : class
  {
    IEntityModel model = EnsureEntity<TEntity>();

    await callback(entity, model, default, state, cancellationToken);
    await _executor.VisitGraphAsync(entity, model, state, callback, cancellationToken);
  }

  private async Task<TEntity?> FindAsyncCore<TEntity, TKey>(TKey key, NavigationSpec<TEntity, TKey> spec, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    DbCommand command = Connection.CreateCommand();
    await Connection.OpenAsync(cancellationToken);
    try
    {
      return await _executor.ExecuteSelectCommandAsync(command, key, in spec, cancellationToken);
    }
    finally
    {
      command.Dispose();
      Connection.Close();
    }
  }

  private IEntityModel<TEntity, TKey> EnsureEntity<TEntity, TKey>()
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (!_model.TryGetEntity(out IEntityModel<TEntity, TKey>? entityModel))
      throw new InvalidOperationException($"'{typeof(TEntity).Name}' is not registered as a database entity.");

    return entityModel;
  }

  private IEntityModel EnsureEntity<TEntity>()
  {
    if (!_model.TryGetEntity(typeof(TEntity), out IEntityModel? entityModel))
      throw new InvalidOperationException($"'{typeof(TEntity).Name}' is not registered as a database entity.");

    return entityModel;
  }
}
