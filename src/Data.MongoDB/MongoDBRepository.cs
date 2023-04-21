using CodeArchitects.Platform.Common.CodeAnalysis;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// A MongoDB implementation of <see cref="Repository{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
[Experimental]
public class MongoDBRepository<TEntity, TKey> : Repository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MongoDBRepository{TEntity, TKey}"/> class.
  /// </summary>
  /// <param name="context">The MongoDB data context used by the repository</param>
  public MongoDBRepository(IDataContext context)
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

  private protected override Data.IDataContext DataContext => Context;
}
