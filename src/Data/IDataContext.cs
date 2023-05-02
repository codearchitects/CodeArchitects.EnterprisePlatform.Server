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
  /// <param name="key">The value of the primary key for the entity to find.</param>
  /// <returns>The found entity, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  TEntity? Find<TEntity, TKey>(TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Finds an entity with the specified primary key value.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="key">The value of the primary key for the entity to find.</param>
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
  /// <param name="key">The value of the primary key for the entity to find.</param>
  /// <param name="includeAction">Specifies which related entities to include in the query.</param>
  /// <returns>The found entity with included related entities, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  TEntity? Find<TEntity, TKey>(TKey key, IncludeAction<TEntity> includeAction)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Finds an entity with the specified primary key value and includes the specified related entities.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="key">The value of the primary key for the entity to find.</param>
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
  /// <param name="entity">The entity to insert.</param>
  void Insert<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Inserts a new entity into the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entity">The entity to insert.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task InsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Inserts new entities into the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entities">The entities to insert.</param>
  void InsertMany<TEntity, TKey>(IEnumerable<TEntity> entities)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Inserts new entities into the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entities">The entities to insert.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task InsertManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Updates an existing entity in the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entity">The entity to update.</param>
  void Update<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Updates an existing entity in the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entity">The entity to update.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task UpdateAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Updates existing entities in the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entities">The entities to update.</param>
  void UpdateMany<TEntity, TKey>(IEnumerable<TEntity> entities)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Updates existing entities in the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entities">The entities to update.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task UpdateManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Inserts or updates an existing entity in the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entity">The entity to insert or update.</param>
  void Upsert<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Inserts or updates an existing entity in the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entity">The entity to insert or update.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task UpsertAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Removes an existing entity from the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entity">The entity to remove.</param>
  void Remove<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Removes an existing entity from the repository.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entity">The entity to remove.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task RemoveAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Removes an existing entity from the repository by its primary key value.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="key">The value of the primary key for the entity to remove.</param>
  void Remove<TEntity, TKey>(TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Removes an existing entity from the repository by its primary key value.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="key">The value of the primary key for the entity to remove.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task RemoveAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
