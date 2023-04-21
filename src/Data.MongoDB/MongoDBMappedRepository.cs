using CodeArchitects.Platform.Common.CodeAnalysis;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// A MongoDB implementation of <see cref="MappedRepository{TCollection, TEntity, TKey}"/>
/// </summary>
/// <typeparam name="TCollection">The collection entity type.</typeparam>
/// <typeparam name="TEntity">The domain entity type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
[Experimental]
public abstract class MongoDBMappedRepository<TCollection, TEntity, TKey> : MappedRepository<TCollection, TEntity, TKey>
  where TCollection : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MongoDBMappedRepository{TCollection, TEntity, TKey}"/> class.
  /// </summary>
  /// <param name="context"></param>
  protected MongoDBMappedRepository(IDataContext context)
  {
    Context = context;
  }

  /// <summary>
  /// The MongoDB data context used by the repository.
  /// </summary>
  protected IDataContext Context { get; }

  /// <summary>
  /// The <see cref="IMongoDatabase"/> used by the repository.
  /// </summary>
  protected IMongoDatabase Database => Context.Database;

  /// <summary>
  /// The <see cref="IMongoCollection{TDocument}"/> of the repository entity.
  /// </summary>
  protected IMongoCollection<TEntity> Collection => Context.GetCollection<TEntity>();

  private protected sealed override Data.IDataContext DataContext => Context;
}
