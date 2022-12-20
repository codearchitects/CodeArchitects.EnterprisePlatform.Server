using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

/// <summary>
/// An Entity Framework Core implementation of <see cref="Repository{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public abstract class EFCoreRepositoryBase<TEntity, TKey> : Repository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  protected override Data.IDataContext Context => ContextCore;

  private protected abstract IDataContext ContextCore { get; }
}

/// <summary>
/// An Entity Framework Core implementation of <see cref="Repository{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public class EFCoreRepository<TEntity, TKey> : EFCoreRepositoryBase<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EFCoreRepository{TEntity, TKey}"/> class.
  /// </summary>
  /// <param name="context">The EFCore data context used by the repository.</param>
  public EFCoreRepository(IDataContext context)
  {
    Context = context;
  }

  /// <summary>
  /// The EFCore data context used by the repository.
  /// </summary>
  protected new IDataContext Context { get; }

  private protected override IDataContext ContextCore => Context;

  /// <summary>
  /// The application <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
  /// </summary>
  protected DbContext DbContext => Context.DbContext;

  /// <summary>
  /// The <see cref="DbSet{TEntity}"/> of the repository entity.
  /// </summary>
  protected DbSet<TEntity> Entities => Context.DbContext.Set<TEntity>();
}
