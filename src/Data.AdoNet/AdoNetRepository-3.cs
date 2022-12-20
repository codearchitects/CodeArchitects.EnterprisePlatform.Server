using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

/// <summary>
/// An ADO.NET implementation of <see cref="Repository{TEntity, TKey}"/> that uses a specific type of database connection.
/// </summary>
/// <typeparam name="TDbConnection">The type of database connection used by this repository.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public class AdoNetRepository<TConnection, TEntity, TKey> : AdoNetRepository<TEntity, TKey>
  where TConnection : DbConnection
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}"/> class.
  /// </summary>
  /// <param name="context">The ADO.NET data context used by the repository.</param>
  public AdoNetRepository(IDataContext<TConnection> context)
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
