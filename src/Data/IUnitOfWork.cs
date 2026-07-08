namespace CodeArchitects.Platform.Data;

/// <summary>
/// Represents a unit of work for managing changes to entities.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
  /// <summary>
  /// Persists all changes made to entities in scope of the unit of work to the data store.
  /// </summary>
  void Save();

  /// <summary>
  /// Persists all changes made to entities in scope of the unit of work to the data store.
  /// </summary>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task SaveAsync(CancellationToken cancellationToken = default);

  // TODO: Add transactions
}
