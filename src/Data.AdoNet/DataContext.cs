using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal abstract class DataContext<TDbConnection, TDbCommand> : IDataContext<TDbConnection>
  where TDbConnection : DbConnection
  where TDbCommand : DbCommand
{
  private readonly IStateManager<TDbConnection> _stateManager;
  private readonly IExecutor<TDbCommand> _executor;
  private readonly ICommandInterceptor<TDbCommand> _interceptor;
  private readonly IDataModel _model;

  public DataContext(
    IStateManager<TDbConnection> stateManager,
    IExecutor<TDbCommand> executor,
    ICommandInterceptor<TDbCommand> interceptor,
    IDataModel model)
  {
    _stateManager = stateManager;
    _executor = executor;
    _interceptor = interceptor;
    _model = model;
  }

  public TDbConnection Connection => _stateManager.Connection;

  IDbConnection IDataContext.Connection => _stateManager.Connection;

  protected abstract TDbCommand CreateCommand(TDbConnection connection);

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

    bool startTransaction = ShouldStartTransaction(model, entity);

    return _stateManager.ExecuteAsync(async (connection, transaction, cancellationToken) =>
    {
      TDbCommand command = CreateCommand(OperationType.Insert, Connection);

      command.Transaction = transaction;
      await _executor.ExecuteInsertAsync(command, entity, model, cancellationToken);
    }, startTransaction, cancellationToken);
  }

  public Task UpdateAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    bool startTransaction = ShouldStartTransaction(model, entity);

    return _stateManager.ExecuteAsync(async (connection, transaction, cancellationToken) =>
    {
      TDbCommand command = CreateCommand(OperationType.Update, Connection);

      command.Transaction = transaction;
      await _executor.ExecuteUpdateAsync(command, entity, model, cancellationToken);
    }, startTransaction, cancellationToken);
  }

  public Task RemoveAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    return _stateManager.ExecuteAsync(async (connection, transaction, cancellationToken) =>
    {
      TDbCommand command = CreateCommand(OperationType.Remove, Connection);

      command.Transaction = transaction;
      await _executor.ExecuteRemoveAsync(command, entity, model, cancellationToken);
    }, false, cancellationToken);
  }

  public Task RemoveAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    return _stateManager.ExecuteAsync(async (connection, transaction, cancellationToken) =>
    {
      TDbCommand command = CreateCommand(OperationType.Remove, Connection);

      command.Transaction = transaction;
      await _executor.ExecuteRemoveAsync(command, key, model, cancellationToken);
    }, false, cancellationToken);
  }

  public Task BatchExecuteAsync(Execution<TDbConnection, DbTransaction> execution, bool startTransaction, CancellationToken cancellationToken = default)
  {
    return _stateManager.ExecuteAsync(execution, startTransaction, cancellationToken);
  }

  public Task BatchExecuteAsync(Execution<IDbConnection, IDbTransaction> execution, bool startTransaction, CancellationToken cancellationToken = default)
  {
    return _stateManager.ExecuteAsync(execution, startTransaction, cancellationToken);
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
    TDbCommand command = CreateCommand(OperationType.Find, Connection);

    await Connection.OpenAsync(cancellationToken);

    try
    {
      return await _executor.ExecuteFindAsync(command, key, in spec, cancellationToken);
    }
    finally
    {
      command.Dispose();
      Connection.Close();
    }
  }

  private TDbCommand CreateCommand(OperationType operation, TDbConnection connection)
  {
    TDbCommand command = CreateCommand(connection);
    _interceptor.OnCommandCreated(operation, command);

    return command;
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

  private static bool ShouldStartTransaction(IEntityModel model, object entity)
  {
    foreach (INavigationModel navigation in model.Navigations)
    {
      if (navigation.HasMember && navigation.GetValue(entity) is not null)
        return true;
    }

    return false;
  }
}
