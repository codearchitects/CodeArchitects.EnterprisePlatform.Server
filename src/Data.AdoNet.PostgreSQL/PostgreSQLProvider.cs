using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.PostgreSQL.Command;
using Npgsql;

namespace CodeArchitects.Platform.Data.AdoNet.PostgreSQL;

/// <summary>
/// The provider for PostgreSQL databases.
/// </summary>
public class PostgreSQLProvider : DatabaseProvider<NpgsqlConnection, NpgsqlCommand>
{
  internal override ISyntaxProvider CreateSyntaxProvider() => new PostgreSQLSyntaxProvider();

  /// <summary>
  /// Specifies the connection string to the database.
  /// </summary>
  /// <param name="connectionString">The connection string.</param>
  /// <returns>The same <see cref="PostgreSQLProvider"/> for further configuration.</returns>
  public PostgreSQLProvider UseConnection(string connectionString)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));

    DelegateConnectionFactory = new DelegateConnectionFactory<NpgsqlConnection>(() => new NpgsqlConnection(connectionString));
    return this;
  }

  /// <summary>
  /// Specifies a factory that creates instances of <see cref="NpgsqlConnection"/>.
  /// </summary>
  /// <param name="connectionFactory">The factory function.</param>
  /// <returns>The same <see cref="PostgreSQLProvider"/> for further configuration.</returns>
  public PostgreSQLProvider UseConnectionFactory(Func<NpgsqlConnection> connectionFactory)
  {
    if (connectionFactory is null)
      throw new ArgumentNullException(nameof(connectionFactory));

    DelegateConnectionFactory = new DelegateConnectionFactory<NpgsqlConnection>(connectionFactory);
    return this;
  }

  /// <summary>
  /// Specifies a factory that creates instances of <see cref="NpgsqlConnection"/>.
  /// </summary>
  /// <param name="connectionFactoryType">The connection factory type. It must implement <see cref="IConnectionFactory{TDbConnection}"/> of <see cref="NpgsqlConnection"/>.</param>
  /// <returns>The same <see cref="PostgreSQLProvider"/> for further configuration.</returns>
  public PostgreSQLProvider UseConnectionFactory(Type connectionFactoryType)
  {
    if (connectionFactoryType is null)
      throw new ArgumentNullException(nameof(connectionFactoryType));

    UseConnectionFactoryCore(connectionFactoryType);
    return this;
  }
}
