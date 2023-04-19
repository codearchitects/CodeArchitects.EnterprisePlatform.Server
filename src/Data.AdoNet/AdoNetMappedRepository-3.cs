using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

/// <summary>
/// An ADO.NET implementation of <see cref="MappedRepository{TTable, TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TTable">The table entity type.</typeparam>
/// <typeparam name="TEntity">The domain entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public abstract class AdoNetMappedRepository<TTable, TEntity, TKey> : MappedRepository<TTable, TEntity, TKey>
  where TTable : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AdoNetMappedRepository{TTable, TEntity, TKey}"/> class.
  /// </summary>
  /// <param name="context">The ADO.NET data context used by the repository.</param>
  protected AdoNetMappedRepository(IDataContext context)
  {
    Context = context;
  }

  /// <summary>
  /// The ADO.NET data context used by the repository.
  /// </summary>
  protected IDataContext Context { get; }

  /// <summary>
  /// The connection used by the repository's data context.
  /// </summary>
  protected IDbConnection Connection => Context.Connection;

  private protected sealed override Data.IDataContext DataContext => Context;
}
