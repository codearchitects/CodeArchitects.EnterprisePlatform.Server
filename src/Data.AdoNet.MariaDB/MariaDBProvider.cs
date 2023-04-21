using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.MariaDB.Command;
using MySqlConnector;

namespace CodeArchitects.Platform.Data.AdoNet.MariaDB;

public class MariaDBProvider : DatabaseProvider<MySqlConnection, MySqlCommand>
{
  private protected override ISyntaxProvider CreateSyntaxProvider() => new MariaDBSyntaxProvider();

  public MariaDBProvider UseConnection(string connectionString)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));

    DelegateConnectionFactory = new DelegateConnectionFactory<MySqlConnection>(() => new MySqlConnection(connectionString));
    return this;
  }

  //public MariaDBProvider UseConnection(string connectionString, Mysql credential)
  //{
  //  if (connectionString is null)
  //    throw new ArgumentNullException(nameof(connectionString));
  //  if (credential is null)
  //    throw new ArgumentNullException(nameof(credential));

  //  DelegateConnectionFactory = new DelegateConnectionFactory<MySqlConnection>(() => new MySqlConnection(connectionString, credential));
  //  return this;
  //}

  public MariaDBProvider UseConnectionFactory(Func<MySqlConnection> connectionFactory)
  {
    if (connectionFactory is null)
      throw new ArgumentNullException(nameof(connectionFactory));

    DelegateConnectionFactory = new DelegateConnectionFactory<MySqlConnection>(connectionFactory);
    return this;
  }

  public MariaDBProvider UseConnectionFactory(Type connectionFactoryType)
  {
    if (connectionFactoryType is null)
      throw new ArgumentNullException(nameof(connectionFactoryType));

    UseConnectionFactoryCore(connectionFactoryType);
    return this;
  }
}
