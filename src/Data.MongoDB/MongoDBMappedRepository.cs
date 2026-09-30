using CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.MongoDB.Model;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using CodeArchitects.Platform.Data.Navigation;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// A MongoDB implementation of <see cref="MappedRepository{TDocument, TEntity, TKey}"/>, for when the
/// persisted document differs from the domain entity.
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

  #region Find

  /// <inheritdoc/>
  public override TEntity? Find(TKey key)
  {
    TDocument? document = Context.Find<TDocument, TKey>(key);
    return document is null ? null : ToEntity(document);
  }

  /// <inheritdoc/>
  public override async Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    TDocument? document = await Context.FindAsync<TDocument, TKey>(key, cancellationToken);
    return document is null ? null : ToEntity(document);
  }

  /// <summary>
  /// Not supported by a mapped repository.
  /// </summary>
  /// <remarks>
  /// The include is written on the domain entity, which is not part of the MongoDB model, so it cannot
  /// be validated against the document. It is not needed either: the document is always loaded whole,
  /// so <see cref="Find(TKey)"/> already returns the embedded parts of the aggregate.
  /// </remarks>
  /// <exception cref="NotSupportedException">Always.</exception>
  public override TEntity? Find(TKey key, IncludeAction<TEntity> includeAction)
  {
    throw IncludeNotSupported();
  }

  /// <inheritdoc cref="Find(TKey, IncludeAction{TEntity})"/>
  public override Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    throw IncludeNotSupported();
  }

  #endregion

  #region Insert

  // The entity receives its key only once the data context accepted the operation (written, or queued in the
  // unit of work): a failure leaves the entity as it was.

  /// <inheritdoc/>
  public override void Insert(TEntity entity)
  {
    TDocument document = ToInsertedDocument(entity, out IEntityModel documentModel);
    Context.Insert<TDocument, TKey>(document);
    MappedKey<TDocument, TEntity, TKey>.AssignKey(documentModel, entity, document);
  }

  /// <inheritdoc/>
  public override async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    TDocument document = ToInsertedDocument(entity, out IEntityModel documentModel);
    await Context.InsertAsync<TDocument, TKey>(document, cancellationToken);
    MappedKey<TDocument, TEntity, TKey>.AssignKey(documentModel, entity, document);
  }

  /// <inheritdoc/>
  public override void InsertMany(IEnumerable<TEntity> entities)
  {
    TDocument[] documents = ToInsertedDocuments(entities, out TEntity[] batch, out IEntityModel documentModel);
    Context.InsertMany<TDocument, TKey>(documents);
    AssignKeys(documentModel, batch, documents);
  }

  /// <inheritdoc/>
  public override async Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
  {
    TDocument[] documents = ToInsertedDocuments(entities, out TEntity[] batch, out IEntityModel documentModel);
    await Context.InsertManyAsync<TDocument, TKey>(documents, cancellationToken);
    AssignKeys(documentModel, batch, documents);
  }

  #endregion

  #region Update

  /// <inheritdoc/>
  public override void Update(TEntity entity)
  {
    Context.Update<TDocument, TKey>(ToDocument(entity));
  }

  /// <inheritdoc/>
  public override async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    await Context.UpdateAsync<TDocument, TKey>(ToDocument(entity), cancellationToken);
  }

  /// <inheritdoc/>
  public override void UpdateMany(IEnumerable<TEntity> entities)
  {
    Context.UpdateMany<TDocument, TKey>(ToDocuments(entities, out _, DocumentModel));
  }

  /// <inheritdoc/>
  public override async Task UpdateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
  {
    await Context.UpdateManyAsync<TDocument, TKey>(ToDocuments(entities, out _, DocumentModel), cancellationToken);
  }

  #endregion

  #region Upsert

  /// <inheritdoc/>
  public override void Upsert(TEntity entity)
  {
    Context.Upsert<TDocument, TKey>(ToDocument(entity));
  }

  /// <inheritdoc/>
  public override async Task UpsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    await Context.UpsertAsync<TDocument, TKey>(ToDocument(entity), cancellationToken);
  }

  #endregion

  #region Remove

  /// <inheritdoc/>
  public override void Remove(TEntity entity)
  {
    Context.Remove<TDocument, TKey>(ToDocument(entity));
  }

  /// <inheritdoc/>
  public override async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    await Context.RemoveAsync<TDocument, TKey>(ToDocument(entity), cancellationToken);
  }

  #endregion

  #region Mapping

  /// <summary>
  /// Maps a document to a domain entity, through <see cref="MappedRepository{TDocument, TEntity, TKey}.TableToEntity(TDocument)"/>.
  /// </summary>
  /// <param name="document">The document.</param>
  /// <returns>The domain entity.</returns>
  /// <exception cref="ArgumentNullException"><paramref name="document"/> is <c>null</c>.</exception>
  /// <exception cref="InvalidOperationException">The mapping returned <c>null</c>.</exception>
  protected TEntity ToEntity(TDocument document)
  {
    if (document is null)
      throw new ArgumentNullException(nameof(document));

    return TableToEntity(document)
      ?? throw new InvalidOperationException(
        $"The mapping from '{typeof(TDocument).Name}' to '{typeof(TEntity).Name}' returned null.");
  }

  /// <summary>
  /// Maps documents to domain entities.
  /// </summary>
  /// <param name="documents">The documents.</param>
  /// <returns>The domain entities, in the same order.</returns>
  protected List<TEntity> ToEntities(IEnumerable<TDocument> documents)
  {
    if (documents is null)
      throw new ArgumentNullException(nameof(documents));

    return [.. documents.Select(ToEntity)];
  }

  /// <summary>
  /// Runs a query written on the document and maps its results to domain entities.
  /// </summary>
  /// <param name="source">
  /// The query, for example <c>Documents.Find(Session, filter)</c> or <c>Documents.Aggregate(Session)</c>.
  /// </param>
  /// <returns>The domain entities, in the order returned by the query.</returns>
  protected List<TEntity> ToEntities(IAsyncCursorSource<TDocument> source)
  {
    if (source is null)
      throw new ArgumentNullException(nameof(source));

    return ToEntities(source.ToList());
  }

  /// <inheritdoc cref="ToEntities(IAsyncCursorSource{TDocument})"/>
  /// <param name="source">
  /// The query, for example <c>Documents.Find(Session, filter)</c> or <c>Documents.Aggregate(Session)</c>.
  /// </param>
  /// <param name="cancellationToken">A token to cancel the query.</param>
  protected async Task<List<TEntity>> ToEntitiesAsync(IAsyncCursorSource<TDocument> source, CancellationToken cancellationToken = default)
  {
    if (source is null)
      throw new ArgumentNullException(nameof(source));

    return ToEntities(await source.ToListAsync(cancellationToken));
  }

  /// <summary>
  /// Maps a domain entity to a document, through <see cref="MappedRepository{TDocument, TEntity, TKey}.EntityToTable(TEntity)"/>,
  /// and verifies that the key is preserved.
  /// </summary>
  /// <param name="entity">The domain entity.</param>
  /// <returns>The document.</returns>
  /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <c>null</c>.</exception>
  /// <exception cref="InvalidOperationException">The mapping returned <c>null</c>, or changed the key.</exception>
  protected TDocument ToDocument(TEntity entity)
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    return MapToDocument(entity, DocumentModel, position: null);
  }

  /// <summary>
  /// The registered model of the document, as the data context uses it. A data context that does not expose its
  /// model, such as a decorator, falls back to the model built from the document type by the same rules.
  /// </summary>
  private IEntityModel DocumentModel => Context is IEntityModelResolver resolver
    ? resolver.GetEntityModel<TDocument>()
    : s_fallbackDocumentModel.Value;

  private static readonly Lazy<IEntityModel> s_fallbackDocumentModel = new(() => EntityModel.Create(typeof(TDocument)));

  private TDocument ToInsertedDocument(TEntity entity, out IEntityModel documentModel)
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    documentModel = DocumentModel;
    TDocument document = MapToDocument(entity, documentModel, position: null);
    MappedKey<TDocument, TEntity, TKey>.GenerateKey(documentModel, document);

    return document;
  }

  private TDocument[] ToInsertedDocuments(IEnumerable<TEntity> entities, out TEntity[] batch, out IEntityModel documentModel)
  {
    documentModel = DocumentModel;
    TDocument[] documents = ToDocuments(entities, out batch, documentModel);

    // Only once the whole batch mapped: a rejected batch leaves every document, and every entity, untouched.
    foreach (TDocument document in documents)
    {
      MappedKey<TDocument, TEntity, TKey>.GenerateKey(documentModel, document);
    }

    return documents;
  }

  private static void AssignKeys(IEntityModel documentModel, TEntity[] batch, TDocument[] documents)
  {
    for (int index = 0; index < batch.Length; index++)
    {
      MappedKey<TDocument, TEntity, TKey>.AssignKey(documentModel, batch[index], documents[index]);
    }
  }

  /// <summary>
  /// Maps the whole batch before anything reaches the data context: a failure on one element never
  /// leaves part of the batch written or queued in the unit of work.
  /// </summary>
  private TDocument[] ToDocuments(IEnumerable<TEntity> entities, out TEntity[] batch, IEntityModel documentModel)
  {
    if (entities is null)
      throw new ArgumentNullException(nameof(entities));

    batch = [.. entities];
    TDocument[] documents = new TDocument[batch.Length];

    for (int index = 0; index < batch.Length; index++)
    {
      TEntity entity = batch[index]
        ?? throw new ArgumentException($"The element at index {index} is null.", nameof(entities));

      documents[index] = MapToDocument(entity, documentModel, $" (element at index {index})");
    }

    return documents;
  }

  // position: where the entity sits in a batch, to name it in the errors; null for a single entity.
  private TDocument MapToDocument(TEntity entity, IEntityModel documentModel, string? position)
  {
    TDocument? mapped;
    try
    {
      mapped = EntityToTable(entity);
    }
    catch (Exception exception) when (position is not null)
    {
      // A single entity keeps the mapper's own exception; in a batch the element has to be named.
      throw new InvalidOperationException(
        $"The mapping from '{typeof(TEntity).Name}' to '{typeof(TDocument).Name}'{position} failed: {exception.Message}", exception);
    }

    TDocument document = mapped
      ?? throw new InvalidOperationException(
        $"The mapping from '{typeof(TEntity).Name}' to '{typeof(TDocument).Name}'{position} returned null.");

    MappedKey<TDocument, TEntity, TKey>.EnsurePreserved(documentModel, entity, document, position);

    return document;
  }

  private static NotSupportedException IncludeNotSupported() => new(
    $"Include is not supported by the mapped repository of '{typeof(TEntity).Name}': the include is written on the " +
    $"domain entity, which cannot be validated against '{typeof(TDocument).Name}'. Use Find(key): the document is " +
    "loaded whole, so the embedded parts of the aggregate are already mapped. Load references to other aggregates " +
    "from their own repository.");

  #endregion
}
