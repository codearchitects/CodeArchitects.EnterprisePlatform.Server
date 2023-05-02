using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
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
  private readonly ICommandBuilder<TDbCommand> _commandBuilder;

  public DataContext(
    IStateManager<TDbConnection> stateManager,
    IExecutor<TDbCommand> executor,
    ICommandInterceptorAggregator<TDbCommand> interceptor,
    INavigationTreeFactory navigationTreeFactory,
    ICommandBuilder<TDbCommand> commandBuilder,
    IDataModel model)
  {
    _stateManager = stateManager;
    _executor = executor;
    _interceptor = interceptor;
    _navigationTreeFactory = navigationTreeFactory;
    _commandBuilder = commandBuilder;
    Model = model;
  }

  public TDbConnection Connection => _stateManager.Connection;

  public IDataModel Model { get; }

  IDbConnection IDataContext.Connection => _stateManager.Connection;

  public TEntity? Find<TEntity, TKey>(TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> entityModel = EnsureEntity<TEntity, TKey>();

    return FindCore(key, _navigationTreeFactory.CreateEmpty(entityModel));
  }

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> entityModel = EnsureEntity<TEntity, TKey>();

    return FindAsyncCore(key, _navigationTreeFactory.CreateEmpty(entityModel), cancellationToken);
  }

  public TEntity? Find<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (includeAction is null)
      throw new ArgumentNullException(nameof(includeAction));

    IEntityModel<TEntity, TKey> entityModel = EnsureEntity<TEntity, TKey>();

    return FindCore(key, _navigationTreeFactory.Create(entityModel, includeAction));
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

  public void Insert<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    bool startTransaction = ShouldStartTransaction(model, entity);

    _stateManager.Execute((connection, transaction, cancellationToken) =>
    {
      using TDbCommand command = CreateCommand(OperationType.Insert, connection);

      command.Transaction = transaction;
      _executor.ExecuteInsert(command, entity, model);

      return Task.CompletedTask;
    }, startTransaction);
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
      await using TDbCommand command = CreateCommand(OperationType.Insert, connection);

      command.Transaction = transaction;
      await _executor.ExecuteInsertAsync(command, entity, model, cancellationToken);
    }, startTransaction, cancellationToken);
  }

  public void InsertMany<TEntity, TKey>(IEnumerable<TEntity> entities)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entities is null)
      throw new ArgumentNullException(nameof(entities));

    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    bool startTransaction = false;
    foreach (TEntity entity in entities)
    {
      if (ShouldStartTransaction(model, entity))
      {
        startTransaction = true;
        break;
      }
    }

    _stateManager.Execute((connection, transaction, cancellationToken) =>
    {
      using TDbCommand command = CreateCommand(OperationType.InsertMany, connection);

      command.Transaction = transaction;
      _executor.ExecuteInsertMany(command, entities, model);

      return Task.CompletedTask;
    }, startTransaction);
  }

  public Task InsertManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entities is null)
      throw new ArgumentNullException(nameof(entities));

    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    bool startTransaction = false;
    foreach (TEntity entity in entities)
    {
      if (ShouldStartTransaction(model, entity))
      {
        startTransaction = true;
        break;
      }
    }

    return _stateManager.ExecuteAsync(async (connection, transaction, cancellationToken) =>
    {
      await using TDbCommand command = CreateCommand(OperationType.InsertMany, connection);

      command.Transaction = transaction;
      await _executor.ExecuteInsertManyAsync(command, entities, model, cancellationToken);
    }, startTransaction, cancellationToken);
  }

  public void Update<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    bool startTransaction = ShouldStartTransaction(model, entity);

    _stateManager.Execute((connection, transaction, cancellationToken) =>
    {
      using TDbCommand command = CreateCommand(OperationType.Update, connection);

      command.Transaction = transaction;
      _executor.ExecuteUpdate(command, entity, model);

      return Task.CompletedTask;
    }, startTransaction);
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
      await using TDbCommand command = CreateCommand(OperationType.Update, connection);

      command.Transaction = transaction;
      await _executor.ExecuteUpdateAsync(command, entity, model, cancellationToken);
    }, startTransaction, cancellationToken);
  }

  public void UpdateMany<TEntity, TKey>(IEnumerable<TEntity> entities)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entities is null)
      throw new ArgumentNullException(nameof(entities));

    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    _stateManager.Execute((connection, transaction, cancellationToken) =>
    {
      using TDbCommand command = CreateCommand(OperationType.UpdateMany, connection);

      command.Transaction = transaction;
      _executor.ExecuteUpdateMany(command, entities, model);

      return Task.CompletedTask;
    }, true);
  }

  public Task UpdateManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entities is null)
      throw new ArgumentNullException(nameof(entities));

    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    return _stateManager.ExecuteAsync(async (connection, transaction, cancellationToken) =>
    {
      await using TDbCommand command = CreateCommand(OperationType.UpdateMany, connection);

      command.Transaction = transaction;
      await _executor.ExecuteUpdateManyAsync(command, entities, model, cancellationToken);
    }, true, cancellationToken);
  }

  public void Upsert<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    bool startTransaction = ShouldStartTransaction(model, entity);

    _stateManager.Execute((connection, transaction, cancellationToken) =>
    {
      using TDbCommand command = CreateCommand(OperationType.Upsert, connection);

      command.Transaction = transaction;
      _executor.ExecuteUpsert(command, entity, model);

      return Task.CompletedTask;
    }, startTransaction);
  }

  public Task UpsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    bool startTransaction = ShouldStartTransaction(model, entity);

    return _stateManager.ExecuteAsync(async (connection, transaction, cancellationToken) =>
    {
      await using TDbCommand command = CreateCommand(OperationType.Upsert, connection);

      command.Transaction = transaction;
      await _executor.ExecuteUpsertAsync(command, entity, model, cancellationToken);
    }, startTransaction, cancellationToken);
  }

  public void Remove<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    _stateManager.Execute((connection, transaction, cancellationToken) =>
    {
      using TDbCommand command = CreateCommand(OperationType.Remove, connection);

      command.Transaction = transaction;
      _executor.ExecuteRemove(command, entity, model);

      return Task.CompletedTask;
    }, false);
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
      await using TDbCommand command = CreateCommand(OperationType.Remove, connection);

      command.Transaction = transaction;
      await _executor.ExecuteRemoveAsync(command, entity, model, cancellationToken);
    }, false, cancellationToken);
  }

  public void Remove<TEntity, TKey>(TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    _stateManager.Execute((connection, transaction, cancellationToken) =>
    {
      using TDbCommand command = CreateCommand(OperationType.Remove, connection);

      command.Transaction = transaction;
      _executor.ExecuteRemove(command, key, model);

      return Task.CompletedTask;
    }, false);
  }

  public Task RemoveAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> model = EnsureEntity<TEntity, TKey>();

    return _stateManager.ExecuteAsync(async (connection, transaction, cancellationToken) =>
    {
      await using TDbCommand command = CreateCommand(OperationType.Remove, connection);

      command.Transaction = transaction;
      await _executor.ExecuteRemoveAsync(command, key, model, cancellationToken);
    }, false, cancellationToken);
  }

  public void BatchExecute(Execution<TDbConnection, DbTransaction> execution, bool startTransaction)
  {
    if (execution is null)
      throw new ArgumentNullException(nameof(execution));

    _stateManager.Execute(execution, startTransaction);
  }

  public Task BatchExecuteAsync(Execution<TDbConnection, DbTransaction> execution, bool startTransaction, CancellationToken cancellationToken = default)
  {
    if (execution is null)
      throw new ArgumentNullException(nameof(execution));

    return _stateManager.ExecuteAsync(execution, startTransaction, cancellationToken);
  }

  public void BatchExecute(Execution<IDbConnection, IDbTransaction> execution, bool startTransaction)
  {
    if (execution is null)
      throw new ArgumentNullException(nameof(execution));

    _stateManager.Execute(execution, startTransaction);
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
    Graph.Visit(entity, model, state, callback);
  }

  public void VisitGraph<TEntity>(TEntity entity, IGraphVisitor visitor) where TEntity : class
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));
    if (visitor is null)
      throw new ArgumentNullException(nameof(visitor));

    IEntityModel model = EnsureEntity<TEntity>();
    Graph.Visit(entity, model, visitor);
  }

  public Task VisitGraphAsync<TEntity, TState>(TEntity entity, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken)
    where TEntity : class
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));
    if (callback is null)
      throw new ArgumentNullException(nameof(callback));

    IEntityModel model = EnsureEntity<TEntity>();
    return Graph.VisitAsync(entity, model, state, callback, cancellationToken);
  }

  public Task VisitGraphAsync<TEntity>(TEntity entity, IAsyncGraphVisitor visitor, CancellationToken cancellationToken) where TEntity : class
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));
    if (visitor is null)
      throw new ArgumentNullException(nameof(visitor));

    IEntityModel model = EnsureEntity<TEntity>();
    return Graph.VisitAsync(entity, model, visitor, cancellationToken);
  }

  public string IncludeNavigations<TEntity, TKey>(string query, IncludeAction<TEntity> includeAction)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> entityModel = EnsureEntity<TEntity, TKey>();

    return _commandBuilder.BuildCustomCommand(query, _navigationTreeFactory.Create(entityModel, includeAction));
  }

  protected virtual TDbCommand CreateCommand(TDbConnection connection)
  {
    return (TDbCommand)connection.CreateCommand();
  }

  private TEntity? FindCore<TEntity, TKey>(TKey key, INavigationRoot<TEntity, TKey> root)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    using TDbCommand command = CreateCommand(OperationType.Find, Connection);

    Connection.Open();

    try
    {
      return _executor.ExecuteFind(command, key, root);
    }
    finally
    {
      command.Dispose();
      Connection.Close();
    }
  }

  private async Task<TEntity?> FindAsyncCore<TEntity, TKey>(TKey key, INavigationRoot<TEntity, TKey> root, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    await using TDbCommand command = CreateCommand(OperationType.Find, Connection);

    await Connection.OpenAsync(cancellationToken);

    try
    {
      return await _executor.ExecuteFindAsync(command, key, root, cancellationToken);
    }
    finally
    {
      await Connection.CloseAsync();
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
