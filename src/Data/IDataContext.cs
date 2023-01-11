using CodeArchitects.Platform.Data.Navigation;

namespace CodeArchitects.Platform.Data;

/// <summary>
/// Provides access to a data store for entities of a specific type.
/// </summary>
public interface IDataContext
{
  /// <summary>
  /// Finds an entity with the specified primary key value.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="key">The value of the primary key for the entity to be found.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Finds an entity with the specified primary key value and includes the specified related entities.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="key">The value of the primary key for the entity to be found.</param>
  /// <param name="includeAction">Specifies which related entities to include in the query.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity with included related entities, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  Task<TEntity?> FindAsync<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Inserts a new entity into the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entity">The entity to be inserted.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task InsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Updates an existing entity in the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entity">The entity to be updated.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task UpdateAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Inserts or updates an existing entity in the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entity">The entity to be inserted or updated.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task UpsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Removes an existing entity from the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entity">The entity to be removed.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task RemoveAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Removes an existing entity from the repository by its primary key value.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="key">The value of the primary key for the entity to be removed.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task RemoveAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
