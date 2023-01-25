using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

/// <summary>
/// An ADO.NET implementation of <see cref="Repository{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public class AdoNetRepository<TEntity, TKey> : Repository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}"/> class.
  /// </summary>
  /// <param name="context">The ADO.NET data context used by the repository.</param>
  public AdoNetRepository(IDataContext context)
  {
    Context = context;
  }

  /// <summary>
  /// The ADO.NET data context used by the repository.
  /// </summary>
  protected new IDataContext Context { get; }

  /// <summary>
  /// The connection used by the repository's data context.
  /// </summary>
  protected IDbConnection Connection => Context.Connection;

  private protected sealed override Data.IDataContext DataContext => Context;
}
