using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

/// <summary>
/// Creates instances of database connections.
/// </summary>
/// <typeparam name="TDbConnection">The type of database connection.</typeparam>
public interface IConnectionFactory<out TDbConnection>
  where TDbConnection : DbConnection
{
  /// <summary>
  /// Creates a new connection to the database.
  /// </summary>
  /// <returns>The created connection.</returns>
  TDbConnection CreateConnection();
}
