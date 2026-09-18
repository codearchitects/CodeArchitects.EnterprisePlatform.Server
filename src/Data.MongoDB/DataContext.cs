using CodeArchitects.Platform.Data.MongoDB.Collections;
using CodeArchitects.Platform.Data.MongoDB.Filters;
using CodeArchitects.Platform.Data.MongoDB.Model;
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

  public TEntity? Find<TEntity, TKey>(TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ById<TEntity, TKey>(entityModel, key);

    return collection.Find(filter).FirstOrDefault();
  }

  public async Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ById<TEntity, TKey>(entityModel, key);

    return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
  }

  public TEntity? Find<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return Find<TEntity, TKey>(key);
  }

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return FindAsync<TEntity, TKey>(key, cancellationToken);
  }

  public void Insert<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();

    _stateManager.Execute((cancellationToken) =>
    {
      collection.InsertOne(entity);
      return Task.CompletedTask;
    });
  }

  public Task InsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();

    return _stateManager.ExecuteAsync(async (cancellationToken) =>
    {
      await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }, cancellationToken);
  }

  public void InsertMany<TEntity, TKey>(IEnumerable<TEntity> entities)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entities is null)
      throw new ArgumentNullException(nameof(entities));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();

    _stateManager.Execute((cancellationToken) =>
    {
      collection.InsertMany(entities);
      return Task.CompletedTask;
    });
  }

  public Task InsertManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entities is null)
      throw new ArgumentNullException(nameof(entities));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();

    return _stateManager.ExecuteAsync(async (cancellationToken) =>
    {
      await collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
    }, cancellationToken);
  }

  public void Update<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ByEntity<TEntity, TKey>(entityModel, entity);

    _stateManager.Execute((cancellationToken) =>
    {
      ReplaceOneResult updateResult = collection.ReplaceOne(filter, entity);

      if (!IsEntityReplaced(updateResult))
        throw new DBConcurrencyException(); // TODO: Message

      return Task.CompletedTask;
    });
  }

  public Task UpdateAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ByEntity<TEntity, TKey>(entityModel, entity);

    return _stateManager.ExecuteAsync(async (cancellationToken) =>
    {
      ReplaceOneResult updateResult = await collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);

      if (!IsEntityReplaced(updateResult))
        throw new DBConcurrencyException(); // TODO: Message
    }, cancellationToken);
  }

  public void UpdateMany<TEntity, TKey>(IEnumerable<TEntity> entities)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    throw new NotSupportedException();
  }

  public Task UpdateManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    throw new NotSupportedException();
  }

  public void Upsert<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ByEntity<TEntity, TKey>(entityModel, entity);

    _stateManager.Execute((cancellationToken) =>
    {
      ReplaceOneResult upsertResult = collection.ReplaceOne(filter, entity, new ReplaceOptions { IsUpsert = true });

      if (!IsEntityUpsert(upsertResult))
        throw new DBConcurrencyException(); // TODO: Message

      return Task.CompletedTask;
    });
  }

  public Task UpsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ByEntity<TEntity, TKey>(entityModel, entity);

    return _stateManager.ExecuteAsync(async (cancellationToken) =>
    {
      ReplaceOneResult upsertResult = await collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = true }, cancellationToken);

      if (!IsEntityUpsert(upsertResult))
        throw new DBConcurrencyException(); // TODO: Message
    }, cancellationToken);
  }

  public void Remove<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ByEntity<TEntity, TKey>(entityModel, entity);

    _stateManager.Execute((cancellationToken) =>
    {
      DeleteResult deleteResult = collection.DeleteOne(filter);

      if (!IsEntityDeleted(deleteResult))
        throw new DBConcurrencyException(); // TODO: Message

      return Task.CompletedTask;
    });
  }

  public Task RemoveAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ByEntity<TEntity, TKey>(entityModel, entity);

    return _stateManager.ExecuteAsync(async (cancellationToken) =>
    {
      DeleteResult deleteResult = await collection.DeleteOneAsync(filter, cancellationToken);

      if (!IsEntityDeleted(deleteResult))
        throw new DBConcurrencyException(); // TODO: Message
    }, cancellationToken);
  }

  public void Remove<TEntity, TKey>(TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ById<TEntity, TKey>(entityModel, key);

    _stateManager.Execute((cancellationToken) =>
    {
      DeleteResult deleteResult = collection.DeleteOne(filter);

      if (!IsEntityDeleted(deleteResult))
        throw new DBConcurrencyException(); // TODO: Message

      return Task.CompletedTask;
    });
  }

  public Task RemoveAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    FilterDefinition<TEntity> filter = _filters.ById<TEntity, TKey>(entityModel, key);

    return _stateManager.ExecuteAsync(async (cancellationToken) =>
    {
      DeleteResult deleteResult = await collection.DeleteOneAsync(filter, cancellationToken);

      if (!IsEntityDeleted(deleteResult))
        throw new DBConcurrencyException(); // TODO: Message
    }, cancellationToken);
  }

  public IMongoCollection<TEntity> GetCollection<TEntity>()
    where TEntity : class
  {
    return _collections.GetCollection<TEntity>();
  }

  private IEntityModel EnsureEntity<TEntity>()
  {
    if (!_model.TryGetEntity(typeof(TEntity), out IEntityModel? entityModel))
      throw new InvalidOperationException($"'{typeof(TEntity).Name}' is not registered as a database entity.");

    return entityModel;
  }


  private static bool IsEntityDeleted(DeleteResult deleteResult)
  {
    return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
  }

  private static bool IsEntityReplaced(ReplaceOneResult updateResult)
  {
    return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
  }

  private static bool IsEntityUpsert(ReplaceOneResult upsertResult)
  {
    return IsEntityReplaced(upsertResult) || upsertResult.IsAcknowledged && upsertResult.UpsertedId is not null;
  }
}
