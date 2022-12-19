using CodeArchitects.Platform.Data.AdoNet.PostgreSQL.Command;
using Npgsql;

namespace CodeArchitects.Platform.Data.AdoNet.PostgreSQL;

public class PostgreSQLProvider : DatabaseProvider
{
  private protected override Type DbConnectionType => typeof(NpgsqlConnection);

  private protected override Type DbCommandType => typeof(NpgsqlCommand);

  internal override Type SyntaxProviderType => typeof(PostgreSQLSyntaxProvider);

  public PostgreSQLProvider UseConnection(string connectionString)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));

    DelegateConnectionFactory = new DelegateConnectionFactory<NpgsqlConnection>(() => new NpgsqlConnection(connectionString));
    return this;
  }

  public PostgreSQLProvider UseConnectionFactory(Func<NpgsqlConnection> connectionFactory)
  {
    if (connectionFactory is null)
      throw new ArgumentNullException(nameof(connectionFactory));

    DelegateConnectionFactory = new DelegateConnectionFactory<NpgsqlConnection>(connectionFactory);
    return this;
  }

  public PostgreSQLProvider UseConnectionFactory(Type connectionFactoryType)
  {
    if (connectionFactoryType is null)
      throw new ArgumentNullException(nameof(connectionFactoryType));

    UseConnectionFactoryCore(connectionFactoryType);
    return this;
  }
}
