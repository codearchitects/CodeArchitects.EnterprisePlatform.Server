namespace CodeArchitects.Platform.Infrastructure.State;

/// <summary>
/// A key-value state store to persist data in a non-transactional way.
/// </summary>
public interface IStateStore
{
  /// <summary>
  /// Tries to retrieve the state object from the store using a given key.
  /// </summary>
  /// <typeparam name="TState">The type of the state to persist.</typeparam>
  /// <param name="key">The state key.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>A task that, when completed, returns the state object if found, or default otherwise.</returns>
  Task<TState?> GetAsync<TState>(string key, CancellationToken cancellationToken = default);

  /// <summary>
  /// Persists a state object to the store associating it with a key.
  /// </summary>
  /// <typeparam name="TState">The type of the state to persist.</typeparam>
  /// <param name="key">The state key.</param>
  /// <param name="state">The state object.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>A task that completes when the state is persisted.</returns>
  Task SaveAsync<TState>(string key, TState state, CancellationToken cancellationToken = default);

  /// <summary>
  /// Deletes the state associated with a given key.
  /// </summary>
  /// <param name="key">The state key.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>A task that completes when the state is deleted.</returns>
  Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}