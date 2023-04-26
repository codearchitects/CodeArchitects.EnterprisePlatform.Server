using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.MySQL.Command;
using MySqlConnector;

namespace CodeArchitects.Platform.Data.AdoNet.MySQL;

public class MySQLProvider : DatabaseProvider<MySqlConnection, MySqlCommand>
{
  private protected override ISyntaxProvider CreateSyntaxProvider() => new MySQLSyntaxProvider();

  public MySQLProvider UseConnection(string connectionString)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));

    DelegateConnectionFactory = new DelegateConnectionFactory<MySqlConnection>(() => new MySqlConnection(connectionString));
    return this;
  }

  public MySQLProvider UseConnectionFactory(Func<MySqlConnection> connectionFactory)
  {
    if (connectionFactory is null)
      throw new ArgumentNullException(nameof(connectionFactory));

    DelegateConnectionFactory = new DelegateConnectionFactory<MySqlConnection>(connectionFactory);
    return this;
  }

  public MySQLProvider UseConnectionFactory(Type connectionFactoryType)
  {
    if (connectionFactoryType is null)
      throw new ArgumentNullException(nameof(connectionFactoryType));

    UseConnectionFactoryCore(connectionFactoryType);
    return this;
  }
}
