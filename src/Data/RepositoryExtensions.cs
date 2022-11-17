namespace CodeArchitects.Platform.Data;

public static class RepositoryExtensions
{
  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2>(this IRepository<TEntity, (TKey1, TKey2)> repository, TKey1 key1, TKey2 key2, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey1 : IEquatable<TKey1>
    where TKey2 : IEquatable<TKey2>
  {
    return repository.FindAsync((key1, key2), cancellationToken);
  }

  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2, TKey3>(this IRepository<TEntity, (TKey1, TKey2, TKey3)> repository, TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey1 : IEquatable<TKey1>
    where TKey2 : IEquatable<TKey2>
    where TKey3 : IEquatable<TKey3>
  {
    return repository.FindAsync((key1, key2, key3), cancellationToken);
  }

  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2, TKey3, TKey4>(this IRepository<TEntity, (TKey1, TKey2, TKey3, TKey4)> repository, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey1 : IEquatable<TKey1>
    where TKey2 : IEquatable<TKey2>
    where TKey3 : IEquatable<TKey3>
    where TKey4 : IEquatable<TKey4>
  {
    return repository.FindAsync((key1, key2, key3, key4), cancellationToken);
  }

  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2>(this IRepository<TEntity, (TKey1, TKey2)> repository, TKey1 key1, TKey2 key2, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey1 : IEquatable<TKey1>
    where TKey2 : IEquatable<TKey2>
  {
    return repository.FindAsync((key1, key2), includeAction, cancellationToken);
  }

  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2, TKey3>(this IRepository<TEntity, (TKey1, TKey2, TKey3)> repository, TKey1 key1, TKey2 key2, TKey3 key3, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey1 : IEquatable<TKey1>
    where TKey2 : IEquatable<TKey2>
    where TKey3 : IEquatable<TKey3>
  {
    return repository.FindAsync((key1, key2, key3), includeAction, cancellationToken);
  }

  public static Task<TEntity?> FindAsync<TEntity, TKey1, TKey2, TKey3, TKey4>(this IRepository<TEntity, (TKey1, TKey2, TKey3, TKey4)> repository, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
    where TEntity : class
    where TKey1 : IEquatable<TKey1>
    where TKey2 : IEquatable<TKey2>
    where TKey3 : IEquatable<TKey3>
    where TKey4 : IEquatable<TKey4>
  {
    return repository.FindAsync((key1, key2, key3, key4), includeAction, cancellationToken);
  }
}
