using CodeArchitects.Platform.Common.CodeAnalysis;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// A MongoDB implementation of <see cref="MappedRepository{TDocument, TEntity, TKey}"/>
/// </summary>
/// <typeparam name="TDocument">The document type stored in the collection.</typeparam>
/// <typeparam name="TEntity">The domain entity type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
[Experimental]
public abstract class MongoDBMappedRepository<TDocument, TEntity, TKey> : MappedRepository<TDocument, TEntity, TKey>
  where TDocument : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MongoDBMappedRepository{TDocument, TEntity, TKey}"/> class.
  /// </summary>
  /// <param name="context">The MongoDB data context used by the repository.</param>
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
  /// The <see cref="IMongoCollection{TDocument}"/> of the repository document.
  /// </summary>
  /// <remarks>
  /// The collection is the one of <typeparamref name="TDocument"/>, not of the domain entity:
  /// only the document type is registered in the model.
  /// </remarks>
  protected IMongoCollection<TDocument> Collection => Context.GetCollection<TDocument>();

  /// <summary>
  /// An alias of <see cref="Collection"/>, for symmetry with the other providers.
  /// </summary>
  protected IMongoCollection<TDocument> Documents => Collection;

  /// <summary>
  /// The session of the current scope. Pass it to custom queries so that they take part in the
  /// same unit of work.
  /// </summary>
  protected IClientSessionHandle Session => Context.Session;

  private protected sealed override Data.IDataContext DataContext => Context;
}
