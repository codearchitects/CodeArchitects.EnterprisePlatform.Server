using CodeArchitects.Platform.Data.MongoDB.Collections;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Applies the registered <see cref="DataSeed"/> instances, in a deterministic order and
/// idempotently.
/// </summary>
internal sealed class Seeder(ICollectionProvider collections, IStateManager stateManager, IMongoDatabase database) : ISeeder
{
  /// <summary>
  /// Holds one document per seeded collection, written in the same transaction as the seed.
  /// </summary>
  internal const string MarkerCollectionName = "__caep_seeding";

  private readonly ICollectionProvider _collections = collections;
  private readonly IStateManager _stateManager = stateManager;
  private readonly IMongoDatabase _database = database;

  /// <summary>
  /// Applies every seed and commits them as a single unit.
  /// </summary>
  public void Apply(IEnumerable<DataSeed> seeds)
  {
    Enqueue(seeds);
    _stateManager.Save();
  }

  /// <summary>
  /// Applies every seed and commits them as a single unit.
  /// </summary>
  public Task ApplyAsync(IEnumerable<DataSeed> seeds, CancellationToken cancellationToken = default)
  {
    Enqueue(seeds);
    return _stateManager.SaveAsync(cancellationToken);
  }

  public void Seed<TEntity>(IEnumerable<TEntity> entities)
    where TEntity : class
  {
    if (entities is null)
      throw new ArgumentNullException(nameof(entities));

    TEntity[] documents = entities as TEntity[] ?? entities.ToArray();
    if (documents.Length == 0)
      return;

    IMongoCollection<TEntity> collection = _collections.GetCollection<TEntity>();
    IMongoCollection<BsonDocument> markers = _database.GetCollection<BsonDocument>(MarkerCollectionName);

    CountOptions countOptions = new() { Limit = 1 };
    InsertManyOptions insertOptions = new() { IsOrdered = true };

    FilterDefinition<BsonDocument> marker =
      Builders<BsonDocument>.Filter.Eq("_id", collection.CollectionNamespace.CollectionName);
    UpdateDefinition<BsonDocument> claim = Builders<BsonDocument>.Update.CurrentDate("seededAt");
    UpdateOptions claimOptions = new() { IsUpsert = true };

    _stateManager.AddExecution(new Execution(
      session =>
      {
        if (collection.CountDocuments(session, FilterDefinition<TEntity>.Empty, countOptions) > 0)
          return;

        // Before the insert, so that a concurrent seed conflicts as early as possible.
        markers.UpdateOne(session, marker, claim, claimOptions);
        collection.InsertMany(session, documents, insertOptions);
      },
      async (session, cancellationToken) =>
      {
        long count = await collection.CountDocumentsAsync(
          session, FilterDefinition<TEntity>.Empty, countOptions, cancellationToken);

        if (count > 0)
          return;

        await markers.UpdateOneAsync(session, marker, claim, claimOptions, cancellationToken);
        await collection.InsertManyAsync(session, documents, insertOptions, cancellationToken);
      }),
      requiresTransaction: true);
  }

  private void Enqueue(IEnumerable<DataSeed> seeds)
  {
    if (seeds is null)
      throw new ArgumentNullException(nameof(seeds));

    foreach (DataSeed seed in Order(seeds))
    {
      seed.Seed(this);
    }
  }

  /// <summary>
  /// Orders the seeds by <see cref="SeedOrderAttribute"/>, then by type name.
  /// </summary>
  internal static IEnumerable<DataSeed> Order(IEnumerable<DataSeed> seeds)
  {
    return seeds
      .OrderBy(seed => seed.GetType().GetCustomAttribute<SeedOrderAttribute>(inherit: false)?.Order ?? 0)
      .ThenBy(seed => seed.GetType().FullName, StringComparer.Ordinal);
  }
}
