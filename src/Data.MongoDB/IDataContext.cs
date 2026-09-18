using CodeArchitects.Platform.Common.CodeAnalysis;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Represents a <see cref="Data.IDataContext"/> that  is based on MongoDB.
/// </summary>
[Experimental]
public interface IDataContext : Data.IDataContext
{
  /// <summary>
  /// The MongoDB database <see cref="IMongoDatabase"/>.
  /// </summary>
  IMongoDatabase Database { get; }

  /// <summary>
  /// The session of the current scope.
  /// </summary>
  /// <remarks>
  /// Pass it to the custom queries of a specialized repository so that they take part in the
  /// same unit of work. An operation issued without it runs on an implicit session, therefore
  /// outside any transaction in progress.
  /// </remarks>
  IClientSessionHandle Session { get; }

  /// <summary>
  /// Gets the MongoDB collection based on the entity type.
  /// </summary>
  /// <typeparam name="TEntity">The MongoDB collection entity type.</typeparam>
  /// <returns>The MongoDB collection.</returns>
  IMongoCollection<TEntity> GetCollection<TEntity>()
    where TEntity : class;

  /// <summary>
  /// Records an arbitrary operation within the current transactional boundary.
  /// </summary>
  /// <param name="execution">The operation to run, receiving the current session.</param>
  /// <param name="requiresTransaction">
  /// <c>true</c> when the operation is only atomic inside a transaction.
  /// </param>
  /// <param name="cancellationToken">A token to cancel the operation.</param>
  Task ExecuteAsync(
    Func<IClientSessionHandle, CancellationToken, Task> execution,
    bool requiresTransaction = false,
    CancellationToken cancellationToken = default);
}
