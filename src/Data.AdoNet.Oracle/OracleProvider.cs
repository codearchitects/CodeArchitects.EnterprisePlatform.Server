using CodeArchitects.Platform.Data.AdoNet.Oracle.Command;
using Oracle.ManagedDataAccess.Client;

namespace CodeArchitects.Platform.Data.AdoNet.Oracle;

public class OracleProvider : DatabaseProvider
{
  internal override Type SyntaxProviderType => typeof(OracleSyntaxProvider);

  private protected override Type DbConnectionType => typeof(OracleConnection);

  private protected override Type DbCommandType => typeof(OracleCommand);

  internal override Type CommandBuilderType => typeof(Command.OracleCommandBuilder);

  public OracleProvider UseConnection(string connectionString)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));

    DelegateConnectionFactory = new DelegateConnectionFactory<OracleConnection>(() => new OracleConnection(connectionString));
    return this;
  }

  public OracleProvider UseConnection(string connectionString, OracleCredential orclCredential)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));
    if (orclCredential is null)
      throw new ArgumentNullException(nameof(orclCredential));

    DelegateConnectionFactory = new DelegateConnectionFactory<OracleConnection>(() => new OracleConnection(connectionString, orclCredential));
    return this;
  }

  public OracleProvider UseConnectionFactory(Func<OracleConnection> connectionFactory)
  {
    if (connectionFactory is null)
      throw new ArgumentNullException(nameof(connectionFactory));

    DelegateConnectionFactory = new DelegateConnectionFactory<OracleConnection>(connectionFactory);
    return this;
  }

  public OracleProvider UseConnectionFactory(Type connectionFactoryType)
  {
    if (connectionFactoryType is null)
      throw new ArgumentNullException(nameof(connectionFactoryType));

    UseConnectionFactoryCore(connectionFactoryType);
    return this;
  }
}
