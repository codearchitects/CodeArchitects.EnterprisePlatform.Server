using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

/// <summary>
/// An Entity Framework Core implementation of <see cref="MappedRepository{TTable, TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TTable">The table entity type.</typeparam>
/// <typeparam name="TEntity">The domain entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public abstract class EFCoreMappedRepository<TTable, TEntity, TKey> : MappedRepository<TTable, TEntity, TKey>
  where TTable : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EFCoreMappedRepository{TTable, TEntity, TKey}"/> class.
  /// </summary>
  /// <param name="context">The EFCore data context used by the repository.</param>
  protected EFCoreMappedRepository(IDataContext context)
  {
    Context = context;
  }

  /// <summary>
  /// The EFCore data context used by the repository.
  /// </summary>
  protected IDataContext Context { get; }

  /// <summary>
  /// The application <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
  /// </summary>
  protected DbContext DbContext => Context.DbContext;

  /// <summary>
  /// The <see cref="DbSet{TTable}"/> of the repository entity.
  /// </summary>
  protected DbSet<TTable> Entities => Context.DbContext.Set<TTable>();

  private protected sealed override Data.IDataContext DataContext => Context;
}
