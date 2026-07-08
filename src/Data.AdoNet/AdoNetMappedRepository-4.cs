using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

/// <summary>
/// An ADO.NET implementation of <see cref="MappedRepository{TTable, TEntity, TKey}"/> that uses a specific type of <see cref="DbConnection"/>.
/// </summary>
/// <typeparam name="TConnection">The type of database connection used by this repository.</typeparam>
/// <typeparam name="TTable">The table entity type.</typeparam>
/// <typeparam name="TEntity">The domain entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public abstract class AdoNetMappedRepository<TConnection, TTable, TEntity, TKey> : AdoNetMappedRepository<TTable, TEntity, TKey>
  where TConnection : DbConnection
  where TTable : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AdoNetMappedRepository{TConnection, TTable, TEntity, TKey}"/> class.
  /// </summary>
  /// <param name="context">The ADO.NET data context used by the repository.</param>
  protected AdoNetMappedRepository(IDataContext<TConnection> context)
    : base(context)
  {
  }

  /// <summary>
  /// The data context used to access the database.
  /// </summary>
  protected new IDataContext<TConnection> Context => (IDataContext<TConnection>)base.Context;

  /// <summary>
  /// The connection used by the repository's data context.
  /// </summary>
  protected new TConnection Connection => Context.Connection;
}
