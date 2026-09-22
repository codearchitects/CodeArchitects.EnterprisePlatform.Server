using CodeArchitects.Platform.Data.MongoDB.Collections;
using MongoDB.Driver;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Applies the registered <see cref="DataSeed"/> instances, in a deterministic order and
/// idempotently.
/// </summary>
internal sealed class Seeder(ICollectionProvider collections, IStateManager stateManager) : ISeeder
{
  private readonly ICollectionProvider _collections = collections;
  private readonly IStateManager _stateManager = stateManager;

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

    CountOptions countOptions = new() { Limit = 1 };
    InsertManyOptions insertOptions = new() { IsOrdered = true };

    _stateManager.AddExecution(new Execution(
      session =>
      {
        if (collection.CountDocuments(session, FilterDefinition<TEntity>.Empty, countOptions) > 0)
          return;

        collection.InsertMany(session, documents, insertOptions);
      },
      async (session, cancellationToken) =>
      {
        long count = await collection.CountDocumentsAsync(
          session, FilterDefinition<TEntity>.Empty, countOptions, cancellationToken);

        if (count > 0)
          return;

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
