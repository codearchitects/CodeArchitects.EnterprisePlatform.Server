using CodeArchitects.Platform.Data.Navigation;

namespace CodeArchitects.Platform.Data;

/// <summary>
/// Represents a repository for managing entities of type <typeparamref name="TEntity"/> with a primary key of type <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public interface IRepository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Finds an entity with the specified primary key value.
  /// </summary>
  /// <param name="key">The value of the primary key for the entity to be found.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default);

  /// <summary>
  /// Finds an entity with the specified primary key value and includes the specified related entities.
  /// </summary>
  /// <param name="key">The value of the primary key for the entity to be found.</param>
  /// <param name="includeAction">Specifies which related entities to include in the query.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity with included related entities, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default);

  /// <summary>
  /// Inserts a new entity into the repository.
  /// </summary>
  /// <param name="entity">The entity to be inserted.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

  /// <summary>
  /// Updates an existing entity in the repository.
  /// </summary>
  /// <param name="entity">The entity to be updated.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

  /// <summary>
  /// Inserts or updates an existing entity in the repository.
  /// </summary>
  /// <param name="entity">The entity to be inserted or updated.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task UpsertAsync(TEntity entity, CancellationToken cancellationToken = default);

  /// <summary>
  /// Removes an existing entity from the repository.
  /// </summary>
  /// <param name="entity">The entity to be removed.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

  /// <summary>
  /// Removes an existing entity from the repository by its primary key value.
  /// </summary>
  /// <param name="key">The value of the primary key for the entity to be removed.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task RemoveAsync(TKey key, CancellationToken cancellationToken = default);
}
