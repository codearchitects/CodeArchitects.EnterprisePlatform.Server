namespace CodeArchitects.Platform.Data;

/// <summary>
/// Provides extension methods for repositories with composite primary keys.
/// </summary>
public static class RepositoryExtensions
{
  /// <summary>
  /// Finds an entity with the specified composite primary key values.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey1">The first component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey2">The second component of the composite primary key type.</typeparam>
  /// <param name="repository">The repository to perform the operation on.</param>
  /// <param name="key1">The value of the first component of the composite primary key for the entity to be found.</param>
  /// <param name="key2">The value of the second component of the composite primary key for the entity to be found.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2>(this IRepository<TEntity, (TKey1, TKey2)> repository, TKey1 key1, TKey2 key2, CancellationToken cancellationToken = default)
    where TEntity : class
  {
    return repository.FindAsync((key1, key2), cancellationToken);
  }

  /// <summary>
  /// Finds an entity with the specified composite primary key values.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey1">The first component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey2">The second component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey3">The third component of the composite primary key type.</typeparam>
  /// <param name="repository">The repository to perform the operation on.</param>
  /// <param name="key1">The value of the first component of the composite primary key for the entity to be found.</param>
  /// <param name="key2">The value of the second component of the composite primary key for the entity to be found.</param>
  /// <param name="key3">The value of the third component of the composite primary key for the entity to be found.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2, TKey3>(this IRepository<TEntity, (TKey1, TKey2, TKey3)> repository, TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = default)
    where TEntity : class
  {
    return repository.FindAsync((key1, key2, key3), cancellationToken);
  }

  /// <summary>
  /// Finds an entity with the specified composite primary key values.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey1">The first component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey2">The second component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey3">The third component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey4">The fourth component of the composite primary key type.</typeparam>
  /// <param name="repository">The repository to perform the operation on.</param>
  /// <param name="key1">The value of the first component of the composite primary key for the entity to be found.</param>
  /// <param name="key2">The value of the second component of the composite primary key for the entity to be found.</param>
  /// <param name="key3">The value of the third component of the composite primary key for the entity to be found.</param>
  /// <param name="key4">The value of the fourth component of the composite primary key for the entity to be found.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2, TKey3, TKey4>(this IRepository<TEntity, (TKey1, TKey2, TKey3, TKey4)> repository, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, CancellationToken cancellationToken = default)
    where TEntity : class
  {
    return repository.FindAsync((key1, key2, key3, key4), cancellationToken);
  }

  /// <summary>
  /// Finds an entity with the specified primary key value and includes the specified related entities.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey1">The first component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey2">The second component of the composite primary key type.</typeparam>
  /// <param name="repository">The repository to perform the operation on.</param>
  /// <param name="key1">The value of the first component of the composite primary key for the entity to be found.</param>
  /// <param name="key2">The value of the second component of the composite primary key for the entity to be found.</param>
  /// <param name="includeAction">Specifies which related entities to include in the query.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity with included related entities, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2>(this IRepository<TEntity, (TKey1, TKey2)> repository, TKey1 key1, TKey2 key2, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
  {
    return repository.FindAsync((key1, key2), includeAction, cancellationToken);
  }

  /// <summary>
  /// Finds an entity with the specified composite primary key values.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey1">The first component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey2">The second component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey3">The third component of the composite primary key type.</typeparam>
  /// <param name="repository">The repository to perform the operation on.</param>
  /// <param name="key1">The value of the first component of the composite primary key for the entity to be found.</param>
  /// <param name="key2">The value of the second component of the composite primary key for the entity to be found.</param>
  /// <param name="key3">The value of the third component of the composite primary key for the entity to be found.</param>
  /// <param name="includeAction">Specifies which related entities to include in the query.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2, TKey3>(this IRepository<TEntity, (TKey1, TKey2, TKey3)> repository, TKey1 key1, TKey2 key2, TKey3 key3, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
  {
    return repository.FindAsync((key1, key2, key3), includeAction, cancellationToken);
  }

  /// <summary>
  /// Finds an entity with the specified composite primary key values.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey1">The first component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey2">The second component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey3">The third component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey4">The fourth component of the composite primary key type.</typeparam>
  /// <param name="repository">The repository to perform the operation on.</param>
  /// <param name="key1">The value of the first component of the composite primary key for the entity to be found.</param>
  /// <param name="key2">The value of the second component of the composite primary key for the entity to be found.</param>
  /// <param name="key3">The value of the third component of the composite primary key for the entity to be found.</param>
  /// <param name="key4">The value of the fourth component of the composite primary key for the entity to be found.</param>
  /// <param name="includeAction">Specifies which related entities to include in the query.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The found entity, or <c>null</c> if an entity with the specified primary key value was not found.</returns>
  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2, TKey3, TKey4>(this IRepository<TEntity, (TKey1, TKey2, TKey3, TKey4)> repository, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
  {
    return repository.FindAsync((key1, key2, key3, key4), includeAction, cancellationToken);
  }


  /// <summary>
  /// Removes an existing entity from the repository by its primary key value.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey1">The first component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey2">The second component of the composite primary key type.</typeparam>
  /// <param name="repository">The repository to perform the operation on.</param>
  /// <param name="key1">The value of the first component of the composite primary key for the entity to be found.</param>
  /// <param name="key2">The value of the second component of the composite primary key for the entity to be found.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  public static Task RemoveAsync<TEntity, TKey1, TKey2>(this IRepository<TEntity, (TKey1, TKey2)> repository, TKey1 key1, TKey2 key2, CancellationToken cancellationToken = default)
    where TEntity : class
  {
    return repository.RemoveAsync((key1, key2), cancellationToken);
  }

  /// <summary>
  /// Removes an existing entity from the repository by its primary key value.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey1">The first component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey2">The second component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey3">The third component of the composite primary key type.</typeparam>
  /// <param name="repository">The repository to perform the operation on.</param>
  /// <param name="key1">The value of the first component of the composite primary key for the entity to be found.</param>
  /// <param name="key2">The value of the second component of the composite primary key for the entity to be found.</param>
  /// <param name="key3">The value of the third component of the composite primary key for the entity to be found.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  public static Task RemoveAsync<TEntity, TKey1, TKey2, TKey3>(this IRepository<TEntity, (TKey1, TKey2, TKey3)> repository, TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = default)
    where TEntity : class
  {
    return repository.RemoveAsync((key1, key2, key3), cancellationToken);
  }

  /// <summary>
  /// Removes an existing entity from the repository by its primary key value.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey1">The first component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey2">The second component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey3">The third component of the composite primary key type.</typeparam>
  /// <typeparam name="TKey4">The fourth component of the composite primary key type.</typeparam>
  /// <param name="repository">The repository to perform the operation on.</param>
  /// <param name="key1">The value of the first component of the composite primary key for the entity to be found.</param>
  /// <param name="key2">The value of the second component of the composite primary key for the entity to be found.</param>
  /// <param name="key3">The value of the third component of the composite primary key for the entity to be found.</param>
  /// <param name="key4">The value of the fourth component of the composite primary key for the entity to be found.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  public static Task RemoveAsync<TEntity, TKey1, TKey2, TKey3, TKey4>(this IRepository<TEntity, (TKey1, TKey2, TKey3, TKey4)> repository, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, CancellationToken cancellationToken = default)
    where TEntity : class
  {
    return repository.RemoveAsync((key1, key2, key3, key4), cancellationToken);
  }
}
