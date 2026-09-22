using CodeArchitects.Platform.Data.MongoDB.Model;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Data.MongoDB.Collections;

/// <summary>
/// The single place where a <see cref="IMongoCollection{TDocument}"/> is materialized.
/// Instances are thread-safe and immutable, so one per entity type is enough.
/// </summary>
internal sealed class CollectionProvider(IMongoDatabase database, IDataModel model) : ICollectionProvider
{
  private readonly IMongoDatabase _database = database;
  private readonly IDataModel _model = model;
  private readonly ConcurrentDictionary<Type, object> _collections = new();

  public IMongoCollection<TEntity> GetCollection<TEntity>()
    where TEntity : class
  {
    return (IMongoCollection<TEntity>)_collections.GetOrAdd(typeof(TEntity), static (type, state) =>
    {
      if (!state.Model.TryGetEntity(type, out IEntityModel? entity))
        throw new InvalidOperationException($"'{type.Name}' is not registered as a MongoDB entity. Mark it with [Collection] and register it through AddEntitiesFrom(assembly) or AddEntity<TEntity>().");

      return state.Database.GetCollection<TEntity>(entity.CollectionName);
    }, (Database: _database, Model: _model));
  }
}
