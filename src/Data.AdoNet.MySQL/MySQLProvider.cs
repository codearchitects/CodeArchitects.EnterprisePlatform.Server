using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.MySQL.Command;
using MySqlConnector;

namespace CodeArchitects.Platform.Data.AdoNet.MySQL;

/// <summary>
/// The provider for MySQL and MariaDB databases.
/// </summary>
public class MySQLProvider : DatabaseProvider<MySqlConnection, MySqlCommand>
{
  private protected override ISyntaxProvider CreateSyntaxProvider() => new MySQLSyntaxProvider();

  /// <summary>
  /// Specifies the connection string to the database.
  /// </summary>
  /// <param name="connectionString">The connection string.</param>
  /// <returns>The same <see cref="MySQLProvider"/> for further configuration.</returns>
  public MySQLProvider UseConnection(string connectionString)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));

    DelegateConnectionFactory = new DelegateConnectionFactory<MySqlConnection>(() => new MySqlConnection(connectionString));
    return this;
  }

  /// <summary>
  /// Specifies a factory that creates instances of <see cref="MySqlConnection"/>.
  /// </summary>
  /// <param name="connectionFactory">The factory function.</param>
  /// <returns>The same <see cref="MySQLProvider"/> for further configuration.</returns>
  public MySQLProvider UseConnectionFactory(Func<MySqlConnection> connectionFactory)
  {
    if (connectionFactory is null)
      throw new ArgumentNullException(nameof(connectionFactory));

    DelegateConnectionFactory = new DelegateConnectionFactory<MySqlConnection>(connectionFactory);
    return this;
  }

  /// <summary>
  /// Specifies a factory that creates instances of <see cref="MySqlConnection"/>.
  /// </summary>
  /// <param name="connectionFactoryType">The connection factory type. It must implement <see cref="IConnectionFactory{TDbConnection}"/> of <see cref="MySqlConnection"/>.</param>
  /// <returns>The same <see cref="MySQLProvider"/> for further configuration.</returns>
  public MySQLProvider UseConnectionFactory(Type connectionFactoryType)
  {
    if (connectionFactoryType is null)
      throw new ArgumentNullException(nameof(connectionFactoryType));

    UseConnectionFactoryCore(connectionFactoryType);
    return this;
  }
}
