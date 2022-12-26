using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.Navigation;
using System.Data;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal class DataContext<TDbConnection, TDbCommand> : IDataContext<TDbConnection>
  where TDbConnection : DbConnection
  where TDbCommand : DbCommand
{
  private readonly IStateManager<TDbConnection> _stateManager;
  private readonly IExecutor<TDbCommand> _executor;
  private readonly ICommandInterceptor<TDbCommand> _interceptor;
  private readonly INavigationTreeFactory _navigationTreeFactory;

  public DataContext(
    IStateManager<TDbConnection> stateManager,
    IExecutor<TDbCommand> executor,
    ICommandInterceptorAggregator<TDbCommand> interceptor,
    INavigationTreeFactory navigationTreeFactory,
    IDataModel model)
  {
    _stateManager = stateManager;
    _executor = executor;
    _interceptor = interceptor;
    _navigationTreeFactory = navigationTreeFactory;
    Model = model;
  }

  public TDbConnection Connection => _stateManager.Connection;

  public IDataModel Model { get; }

  IDbConnection IDataContext.Connection => _stateManager.Connection;

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> entityModel = EnsureEntity<TEntity, TKey>();

    return FindAsyncCore(key, _navigationTreeFactory.CreateEmpty(entityModel), cancellationToken);
  }

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (includeAction is null)
      throw new ArgumentNullException(nameof(includeAction));

    IEntityModel<TEntity, TKey> entityModel = EnsureEntity<TEntity, TKey>();

    return FindAsyncCore(key, _navigationTreeFactory.Create(entityModel, includeAction), cancellationToken);
  }

  public Task InsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

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
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

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
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

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
    if (execution is null)
      throw new ArgumentNullException(nameof(execution));

    return _stateManager.ExecuteAsync(execution, startTransaction, cancellationToken);
  }

  public Task BatchExecuteAsync(Execution<IDbConnection, IDbTransaction> execution, bool startTransaction, CancellationToken cancellationToken = default)
  {
    if (execution is null)
      throw new ArgumentNullException(nameof(execution));

    return _stateManager.ExecuteAsync(execution, startTransaction, cancellationToken);
  }

  public void VisitGraph<TEntity, TState>(TEntity entity, TState state, VisitNodeCallback<TState> callback)
    where TEntity : class
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));
    if (callback is null)
      throw new ArgumentNullException(nameof(callback));

    IEntityModel model = EnsureEntity<TEntity>();

    callback(entity, model, default, state);
    _executor.VisitGraph(entity, model, state, callback);
  }

  public async Task VisitGraphAsync<TEntity, TState>(TEntity entity, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken)
    where TEntity : class
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));
    if (callback is null)
      throw new ArgumentNullException(nameof(callback));

    IEntityModel model = EnsureEntity<TEntity>();

    await callback(entity, model, default, state, cancellationToken);
    await _executor.VisitGraphAsync(entity, model, state, callback, cancellationToken);
  }

  protected virtual TDbCommand CreateCommand(TDbConnection connection)
  {
    return (TDbCommand)connection.CreateCommand();
  }

  private async Task<TEntity?> FindAsyncCore<TEntity, TKey>(TKey key, INavigationRoot<TEntity, TKey> root, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    TDbCommand command = CreateCommand(OperationType.Find, Connection);

    await Connection.OpenAsync(cancellationToken);

    try
    {
      return await _executor.ExecuteFindAsync(command, key, root, cancellationToken);
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
    if (!Model.TryGetEntity(out IEntityModel<TEntity, TKey>? entityModel))
      throw new InvalidOperationException($"'{typeof(TEntity).Name}' is not registered as a database entity.");

    return entityModel;
  }

  private IEntityModel EnsureEntity<TEntity>()
  {
    if (!Model.TryGetEntity(typeof(TEntity), out IEntityModel? entityModel))
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
