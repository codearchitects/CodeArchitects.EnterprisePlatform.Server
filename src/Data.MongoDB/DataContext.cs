using CodeArchitects.Platform.Data.MongoDB.Model;
using CodeArchitects.Platform.Data.MongoDB.Query;
using CodeArchitects.Platform.Data.Navigation;
using MongoDB.Driver;
using System.Data;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB;

internal class DataContext : IDataContext
{
  private readonly IStateManager _stateManager;
  private readonly IPredicateProvider _predicateProvider;
  private readonly IDataModel _model;

  public DataContext(
    IPredicateProvider predicateProvider,
    IStateManager stateManager,
    IDataModel dataModel,
    IMongoDatabase database)
  {
    _predicateProvider = predicateProvider;
    _stateManager = stateManager;
    _model = dataModel;
    Database = database;
  }

  public IMongoDatabase Database { get; }

  public async Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = GetCollection<TEntity>(entityModel);
    Expression<Func<TEntity, bool>> filter = _predicateProvider.GetPredicate<TEntity, TKey>(key, entityModel);

    return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
  }

  public Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return FindAsync<TEntity, TKey>(key, cancellationToken);
  }

  public Task InsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = GetCollection<TEntity>(entityModel);

    return _stateManager.ExecuteAsync(async (cancellationToken) =>
    {
      await collection.InsertOneAsync(entity, cancellationToken: cancellationToken, options: null);
    }, cancellationToken);
  }

  public Task UpdateAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = GetCollection<TEntity>(entityModel);
    Expression<Func<TEntity, bool>> filter = _predicateProvider.GetPredicate(entity, entityModel);

    return _stateManager.ExecuteAsync(async (cancellationToken) =>
    {
      ReplaceOneResult updateResult = await collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);

      if (!IsEntityReplaced(updateResult))
        throw new DBConcurrencyException(); // TODO: Message
    }, cancellationToken);
  }

  public Task UpsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = GetCollection<TEntity>(entityModel);
    Expression<Func<TEntity, bool>> filter = _predicateProvider.GetPredicate(entity, entityModel);

    return _stateManager.ExecuteAsync(async (cancellationToken) =>
    {
      ReplaceOneResult upsertResult = await collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = true }, cancellationToken);

      if (!IsEntityUpsert(upsertResult))
        throw new DBConcurrencyException(); // TODO: Message
    }, cancellationToken);
  }

  public Task RemoveAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = GetCollection<TEntity>(entityModel);
    Expression<Func<TEntity, bool>> filter = _predicateProvider.GetPredicate<TEntity, TKey>(key, entityModel);

    return _stateManager.ExecuteAsync(async (cancellationToken) =>
    {
      DeleteResult deleteResult = await collection.DeleteOneAsync(filter, cancellationToken);

      if (!IsEntityDeleted(deleteResult))
        throw new DBConcurrencyException(); // TODO: Message
    }, cancellationToken);
  }

  public Task RemoveAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    IEntityModel entityModel = EnsureEntity<TEntity>();

    IMongoCollection<TEntity> collection = GetCollection<TEntity>(entityModel);
    Expression<Func<TEntity, bool>> filter = _predicateProvider.GetPredicate(entity, entityModel);

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
    IEntityModel entityModel = EnsureEntity<TEntity>();

    return GetCollection<TEntity>(entityModel);
  }

  private IEntityModel EnsureEntity<TEntity>()
  {
    if (!_model.TryGetEntity(typeof(TEntity), out IEntityModel? entityModel))
      throw new InvalidOperationException($"'{typeof(TEntity).Name}' is not registered as a database entity.");

    return entityModel;
  }

  private IMongoCollection<TEntity> GetCollection<TEntity>(IEntityModel entityModel)
    where TEntity : class
  {
    return Database.GetCollection<TEntity>(entityModel.CollectionName);
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
