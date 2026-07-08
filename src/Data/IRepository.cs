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
  /// <param name="key">The value of the primary key for the entity to find.</param>
  /// <returns>The found entity, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  TEntity? Find(TKey key);

  /// <summary>
  /// Finds an entity with the specified primary key value.
  /// </summary>
  /// <param name="key">The value of the primary key for the entity to find.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default);

  /// <summary>
  /// Finds an entity with the specified primary key value and includes the specified related entities.
  /// </summary>
  /// <param name="key">The value of the primary key for the entity to find.</param>
  /// <param name="includeAction">Specifies which related entities to include in the query.</param>
  /// <returns>The found entity with included related entities, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  TEntity? Find(TKey key, IncludeAction<TEntity> includeAction);

  /// <summary>
  /// Finds an entity with the specified primary key value and includes the specified related entities.
  /// </summary>
  /// <param name="key">The value of the primary key for the entity to find.</param>
  /// <param name="includeAction">Specifies which related entities to include in the query.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity with included related entities, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default);

  /// <summary>
  /// Inserts a new entity into the repository.
  /// </summary>
  /// <param name="entity">The entity to insert.</param>
  void Insert(TEntity entity);

  /// <summary>
  /// Inserts a new entity into the repository.
  /// </summary>
  /// <param name="entity">The entity to insert.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

  /// <summary>
  /// Inserts new entities into the repository.
  /// </summary>
  /// <param name="entities">The entities to insert.</param>
  void InsertMany(IEnumerable<TEntity> entities);

  /// <summary>
  /// Inserts new entities into the repository.
  /// </summary>
  /// <param name="entities">The entities to insert.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

  /// <summary>
  /// Updates an existing entity in the repository.
  /// </summary>
  /// <param name="entity">The entity to update.</param>
  void Update(TEntity entity);

  /// <summary>
  /// Updates an existing entity in the repository.
  /// </summary>
  /// <param name="entity">The entity to update.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

  /// <summary>
  /// Updates existing entities in the repository.
  /// </summary>
  /// <param name="entities">The entities to update.</param>
  void UpdateMany(IEnumerable<TEntity> entities);

  /// <summary>
  /// Updates existing entities in the repository.
  /// </summary>
  /// <param name="entities">The entities to update.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task UpdateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

  /// <summary>
  /// Inserts or updates an existing entity in the repository.
  /// </summary>
  /// <param name="entity">The entity to insert or update.</param>
  void Upsert(TEntity entity);

  /// <summary>
  /// Inserts or updates an existing entity in the repository.
  /// </summary>
  /// <param name="entity">The entity to insert or update.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task UpsertAsync(TEntity entity, CancellationToken cancellationToken = default);

  /// <summary>
  /// Removes an existing entity from the repository.
  /// </summary>
  /// <param name="entity">The entity to remove.</param>
  void Remove(TEntity entity);

  /// <summary>
  /// Removes an existing entity from the repository.
  /// </summary>
  /// <param name="entity">The entity to remove.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

  /// <summary>
  /// Removes an existing entity from the repository by its primary key value.
  /// </summary>
  /// <param name="key">The value of the primary key for the entity to remove.</param>
  void Remove(TKey key);

  /// <summary>
  /// Removes an existing entity from the repository by its primary key value.
  /// </summary>
  /// <param name="key">The value of the primary key for the entity to remove.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task RemoveAsync(TKey key, CancellationToken cancellationToken = default);
}
