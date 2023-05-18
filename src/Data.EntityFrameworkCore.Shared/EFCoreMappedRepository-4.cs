using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

/// <summary>
/// An Entity Framework Core implementation of <see cref="MappedRepository{TTable, TEntity, TKey}"/> that uses a specific <see cref="Microsoft.EntityFrameworkCore.DbContext"/> type.
/// </summary>
/// <typeparam name="TDbContext">The DB context type.</typeparam>
/// <typeparam name="TTable">The table entity type.</typeparam>
/// <typeparam name="TEntity">The domain entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public abstract class EFCoreMappedRepository<TDbContext, TTable, TEntity, TKey> : EFCoreMappedRepository<TTable, TEntity, TKey>
  where TDbContext : DbContext
  where TTable : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EFCoreMappedRepository{TDbContect, TTable, TEntity, TKey}"/> class.
  /// </summary>
  /// <param name="context">The EFCore data context used by the repository.</param>
  protected EFCoreMappedRepository(IDataContext<TDbContext> context)
    : base(context)
  {
  }

  /// <summary>
  /// The EFCore data context used by the repository.
  /// </summary>
  protected new IDataContext<TDbContext> Context => (IDataContext<TDbContext>)base.Context;

  /// <summary>
  /// The application <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
  /// </summary>
  protected new TDbContext DbContext => Context.DbContext;
}
