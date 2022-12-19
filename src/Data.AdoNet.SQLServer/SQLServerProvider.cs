using CodeArchitects.Platform.Data.AdoNet.SQLServer.Command;
using Microsoft.Data.SqlClient;

namespace CodeArchitects.Platform.Data.AdoNet.SQLServer;

public class SQLServerProvider : DatabaseProvider
{
  private protected override Type DbConnectionType => typeof(SqlConnection);

  private protected override Type DbCommandType => typeof(SqlCommand);

  internal override Type SyntaxProviderType => typeof(SQLServerSyntaxProvider);

  public SQLServerProvider UseConnection(string connectionString)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));

    DelegateConnectionFactory = new DelegateConnectionFactory<SqlConnection>(() => new SqlConnection(connectionString));
    return this;
  }

  public SQLServerProvider UseConnection(string connectionString, SqlCredential credential)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));
    if (credential is null)
      throw new ArgumentNullException(nameof(credential));

    DelegateConnectionFactory = new DelegateConnectionFactory<SqlConnection>(() => new SqlConnection(connectionString, credential));
    return this;
  }

  public SQLServerProvider UseConnectionFactory(Func<SqlConnection> connectionFactory)
  {
    if (connectionFactory is null)
      throw new ArgumentNullException(nameof(connectionFactory));

    DelegateConnectionFactory = new DelegateConnectionFactory<SqlConnection>(connectionFactory);
    return this;
  }

  public SQLServerProvider UseConnectionFactory(Type connectionFactoryType)
  {
    if (connectionFactoryType is null)
      throw new ArgumentNullException(nameof(connectionFactoryType));

    UseConnectionFactoryCore(connectionFactoryType);
    return this;
  }
}
