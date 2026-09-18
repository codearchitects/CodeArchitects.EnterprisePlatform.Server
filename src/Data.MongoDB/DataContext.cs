using CodeArchitects.Platform.Data.MongoDB.Collections;
using CodeArchitects.Platform.Data.MongoDB.Filters;
using CodeArchitects.Platform.Data.MongoDB.Model;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using CodeArchitects.Platform.Data.Navigation;
using MongoDB.Driver;
using System.Data;

namespace CodeArchitects.Platform.Data.MongoDB;

internal class DataContext : IDataContext
{
  private readonly IStateManager _stateManager;
  private readonly IFilterProvider _filters;
  private readonly ICollectionProvider _collections;
  private readonly IDataModel _model;

  public DataContext(
    IFilterProvider filterProvider,
    ICollectionProvider collectionProvider,
    IStateManager stateManager,
    IDataModel dataModel,
    IMongoDatabase database)
  {
    _filters = filterProvider;
    _collections = collectionProvider;
    _stateManager = stateManager;
    _model = dataModel;
    Database = database;
  }

  public IMongoDatabase Database { get; }

  public IClientSessionHandle Session => _stateManager.Session;

  #region Find

  public TEntity? Find<TEntity, TKey>(TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel entityModel = EnsureEntity<TEntity>();

    return _collections.GetCollection<TEntity>()
      .Find(Session, _filters.ById<TEntity, TKey>(entityModel, key))
      .FirstOrDefault();
  }

  public async Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel entityModel = EnsureEntity<TEntity>();

    return await _collections.GetCollection<TEntity>()
      .Find(Session, _filters.ById<TEntity, TKey>(entityModel, key))
      .FirstOrDefaultAsync(cancellationToken);
  }

  public TEntity? Find<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    throw IncludeNotSupported<TEntity>();
  }

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    throw IncludeNotSupported<TEntity>();
  }

  #endregion

  #region Insert

  public void Insert<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _stateManager.Execute(InsertExecution<TEntity, TKey>(entity), requiresTransaction: false);
  }

  public Task InsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return _stateManager.ExecuteAsync(InsertExecution<TEntity, TKey>(entity), requiresTransaction: false, cancellationToken);
  }

  public void InsertMany<TEntity, TKey>(IEnumerable<TEntity> entities)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _stateManager.Execute(InsertManyExecution<TEntity, TKey>(entities), requiresTransaction: true);
  }

  public Task InsertManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return _stateManager.ExecuteAsync(InsertManyExecution<TEntity, TKey>(entities), requiresTransaction: true, cancellationToken);
  }

  private Execution InsertExecution<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    _ = EnsureEntity<TEntity>();
    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();

    return new Execution(
      session => collection.InsertOne(session, entity),
      (session, cancellationToken) => collection.InsertOneAsync(session, entity, cancellationToken: cancellationToken));
  }

  private Execution InsertManyExecution<TEntity, TKey>(IEnumerable<TEntity> entities)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entities is null)
      throw new ArgumentNullException(nameof(entities));

    _ = EnsureEntity<TEntity>();
    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    TEntity[] documents = entities as TEntity[] ?? entities.ToArray();

    InsertManyOptions options = new() { IsOrdered = true };

    return new Execution(
      session => collection.InsertMany(session, documents, options),
      (session, cancellationToken) => collection.InsertManyAsync(session, documents, options, cancellationToken));
  }

  #endregion

  #region Update

  public void Update<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _stateManager.Execute(UpdateExecution<TEntity, TKey>(entity), requiresTransaction: false);
  }

  public Task UpdateAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return _stateManager.ExecuteAsync(UpdateExecution<TEntity, TKey>(entity), requiresTransaction: false, cancellationToken);
  }

  public void UpdateMany<TEntity, TKey>(IEnumerable<TEntity> entities)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _stateManager.Execute(UpdateManyExecution<TEntity, TKey>(entities), requiresTransaction: true);
  }

  public Task UpdateManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return _stateManager.ExecuteAsync(UpdateManyExecution<TEntity, TKey>(entities), requiresTransaction: true, cancellationToken);
  }

  private Execution UpdateExecution<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();
    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ByEntity<TEntity, TKey>(entityModel, entity);

    return new Execution(
      session => EnsureUpdated<TEntity, TKey>(collection.ReplaceOne(session, filter, entity), entityModel, entity),
      async (session, cancellationToken) => EnsureUpdated<TEntity, TKey>(
        await collection.ReplaceOneAsync(session, filter, entity, cancellationToken: cancellationToken), entityModel, entity));
  }

  private Execution UpdateManyExecution<TEntity, TKey>(IEnumerable<TEntity> entities)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entities is null)
      throw new ArgumentNullException(nameof(entities));

    IEntityModel entityModel = EnsureEntity<TEntity>();
    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();

    // A single BulkWrite instead of N ReplaceOne: one round-trip, and an aggregate
    // MatchedCount to verify that every document existed.
    ReplaceOneModel<TEntity>[] requests = entities
      .Select(entity => new ReplaceOneModel<TEntity>(_filters.ByEntity<TEntity, TKey>(entityModel, entity), entity))
      .ToArray();

    BulkWriteOptions options = new() { IsOrdered = true };

    return new Execution(
      session =>
      {
        if (requests.Length == 0)
          return;

        EnsureAllUpdated(collection.BulkWrite(session, requests, options), requests.Length, entityModel);
      },
      async (session, cancellationToken) =>
      {
        if (requests.Length == 0)
          return;

        EnsureAllUpdated(
          await collection.BulkWriteAsync(session, requests, options, cancellationToken), requests.Length, entityModel);
      });
  }

  #endregion

  #region Upsert

  public void Upsert<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _stateManager.Execute(UpsertExecution<TEntity, TKey>(entity), requiresTransaction: false);
  }

  public Task UpsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return _stateManager.ExecuteAsync(UpsertExecution<TEntity, TKey>(entity), requiresTransaction: false, cancellationToken);
  }

  private Execution UpsertExecution<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();
    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ByEntity<TEntity, TKey>(entityModel, entity);

    ReplaceOptions options = new() { IsUpsert = true };

    return new Execution(
      session => EnsureUpserted<TEntity, TKey>(collection.ReplaceOne(session, filter, entity, options), entityModel, entity),
      async (session, cancellationToken) => EnsureUpserted<TEntity, TKey>(
        await collection.ReplaceOneAsync(session, filter, entity, options, cancellationToken), entityModel, entity));
  }

  #endregion

  #region Remove

  public void Remove<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _stateManager.Execute(RemoveByEntityExecution<TEntity, TKey>(entity), requiresTransaction: false);
  }

  public Task RemoveAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return _stateManager.ExecuteAsync(RemoveByEntityExecution<TEntity, TKey>(entity), requiresTransaction: false, cancellationToken);
  }

  public void Remove<TEntity, TKey>(TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _stateManager.Execute(RemoveByKeyExecution<TEntity, TKey>(key), requiresTransaction: false);
  }

  public Task RemoveAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return _stateManager.ExecuteAsync(RemoveByKeyExecution<TEntity, TKey>(key), requiresTransaction: false, cancellationToken);
  }

  private Execution RemoveByEntityExecution<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    return RemoveExecution<TEntity>(entityModel, KeyAccessor.Get<TEntity, TKey>(entityModel, entity),
      _filters.ByEntity<TEntity, TKey>(entityModel, entity));
  }

  private Execution RemoveByKeyExecution<TEntity, TKey>(TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel entityModel = EnsureEntity<TEntity>();

    return RemoveExecution<TEntity>(entityModel, key, _filters.ById<TEntity, TKey>(entityModel, key));
  }

  private Execution RemoveExecution<TEntity>(IEntityModel entityModel, object? key, FilterDefinition<TEntity> filter)
    where TEntity : class
  {
    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();

    return new Execution(
      session => EnsureRemoved(collection.DeleteOne(session, filter), entityModel, key),
      async (session, cancellationToken) => EnsureRemoved(
        await collection.DeleteOneAsync(session, filter, options: null, cancellationToken), entityModel, key));
  }

  #endregion

  public IMongoCollection<TEntity> GetCollection<TEntity>()
    where TEntity : class
  {
    return _collections.GetCollection<TEntity>();
  }

  public Task ExecuteAsync(
    Func<IClientSessionHandle, CancellationToken, Task> execution,
    bool requiresTransaction = false,
    CancellationToken cancellationToken = default)
  {
    if (execution is null)
      throw new ArgumentNullException(nameof(execution));

    return _stateManager.ExecuteAsync(
      new Execution(
        session => execution(session, CancellationToken.None).GetAwaiter().GetResult(),
        execution),
      requiresTransaction,
      cancellationToken);
  }

  private IEntityModel EnsureEntity<TEntity>()
  {
    if (!_model.TryGetEntity(typeof(TEntity), out IEntityModel? entityModel))
      throw new InvalidOperationException(
        $"'{typeof(TEntity).Name}' is not registered as a MongoDB entity. Mark it with [Collection] " +
        $"and register it through AddEntitiesFrom(assembly) or AddEntity<TEntity>().");

    return entityModel;
  }

  private static NotSupportedException IncludeNotSupported<TEntity>()
  {
    return new NotSupportedException(
      $"The MongoDB provider does not support Find/FindAsync with Include yet (entity '{typeof(TEntity).Name}'). " +
      "Intra-aggregate associations are already embedded in the document and need no Include; " +
      "for inter-aggregate references, query the target collection explicitly.");
  }

  /// <summary>
  /// A replace that matched but changed nothing reports ModifiedCount == 0: an idempotent update
  /// is a success, so the criterion is MatchedCount.
  /// </summary>
  private static void EnsureUpdated<TEntity, TKey>(ReplaceOneResult result, IEntityModel entityModel, TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (result.IsAcknowledged && result.MatchedCount > 0)
      return;

    throw new DBConcurrencyException(
      $"Update of '{entityModel.Type.Name}' with key '{KeyAccessor.Get<TEntity, TKey>(entityModel, entity)}' " +
      $"was not applied: no matching document in collection '{entityModel.CollectionName}'.");
  }

  private static void EnsureAllUpdated(BulkWriteResult result, int expected, IEntityModel entityModel)
  {
    if (result.IsAcknowledged && result.MatchedCount == expected)
      return;

    throw new DBConcurrencyException(
      $"Update of {expected} '{entityModel.Type.Name}' documents was not applied: " +
      $"{expected - result.MatchedCount} of them have no matching document in collection " +
      $"'{entityModel.CollectionName}'.");
  }

  private static void EnsureUpserted<TEntity, TKey>(ReplaceOneResult result, IEntityModel entityModel, TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (result.IsAcknowledged && (result.MatchedCount > 0 || result.UpsertedId is not null))
      return;

    throw new DBConcurrencyException(
      $"Upsert of '{entityModel.Type.Name}' with key '{KeyAccessor.Get<TEntity, TKey>(entityModel, entity)}' " +
      $"was not applied in collection '{entityModel.CollectionName}'.");
  }

  private static void EnsureRemoved(DeleteResult result, IEntityModel entityModel, object? key)
  {
    if (result.IsAcknowledged && result.DeletedCount > 0)
      return;

    throw new DBConcurrencyException(
      $"Removal of '{entityModel.Type.Name}' with key '{key}' was not applied: " +
      $"no matching document in collection '{entityModel.CollectionName}'.");
  }
}
