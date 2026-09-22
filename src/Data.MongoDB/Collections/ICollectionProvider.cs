using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB.Collections;

/// <summary>
/// Resolves the <see cref="IMongoCollection{TDocument}"/> of a registered entity type.
/// </summary>
internal interface ICollectionProvider
{
  IMongoCollection<TEntity> GetCollection<TEntity>()
    where TEntity : class;
}
