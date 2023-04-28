using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.Features.Concurrency;
using CodeArchitects.Platform.Data.AdoNet.Oracle.Command;
using Oracle.ManagedDataAccess.Client;

namespace CodeArchitects.Platform.Data.AdoNet.Oracle;

/// <summary>
/// The Oracle database provider.
/// </summary>
public class OracleProvider : DatabaseProvider<OracleConnection, OracleCommand>
{
  private protected override ISyntaxProvider CreateSyntaxProvider() => new OracleSyntaxProvider();

  private protected override CommandBuilder<OracleCommand> CreateCommandBuilder(ISqlTextBuilder sqlBuilder, IConcurrencyContext concurrencyContext)
  {
    return new Command.OracleCommandBuilder(sqlBuilder, concurrencyContext);
  }

  /// <summary>
  /// Specifies the connection string to the database.
  /// </summary>
  /// <param name="connectionString">The connection string.</param>
  /// <returns>The same <see cref="OracleProvider"/> for further configuration.</returns>
  public OracleProvider UseConnection(string connectionString)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));

    DelegateConnectionFactory = new DelegateConnectionFactory<OracleConnection>(() => new OracleConnection(connectionString));
    return this;
  }

  /// <summary>
  /// Specifies the connection string to the database.
  /// </summary>
  /// <param name="connectionString">The connection string.</param>
  /// <param name="orclCredential">The credentials to use for authentication.</param>
  /// <returns>The same <see cref="OracleProvider"/> for further configuration.</returns>
  public OracleProvider UseConnection(string connectionString, OracleCredential orclCredential)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));
    if (orclCredential is null)
      throw new ArgumentNullException(nameof(orclCredential));

    DelegateConnectionFactory = new DelegateConnectionFactory<OracleConnection>(() => new OracleConnection(connectionString, orclCredential));
    return this;
  }

  /// <summary>
  /// Specifies a factory that creates instances of <see cref="OracleConnection"/>.
  /// </summary>
  /// <param name="connectionFactory">The factory function.</param>
  /// <returns>The same <see cref="OracleProvider"/> for further configuration.</returns>
  public OracleProvider UseConnectionFactory(Func<OracleConnection> connectionFactory)
  {
    if (connectionFactory is null)
      throw new ArgumentNullException(nameof(connectionFactory));

    DelegateConnectionFactory = new DelegateConnectionFactory<OracleConnection>(connectionFactory);
    return this;
  }

  /// <summary>
  /// Specifies a factory that creates instances of <see cref="OracleConnection"/>.
  /// </summary>
  /// <param name="connectionFactoryType">The connection factory type. It must implement <see cref="IConnectionFactory{TDbConnection}"/> of <see cref="OracleConnection"/>.</param>
  /// <returns>The same <see cref="OracleProvider"/> for further configuration.</returns>
  public OracleProvider UseConnectionFactory(Type connectionFactoryType)
  {
    if (connectionFactoryType is null)
      throw new ArgumentNullException(nameof(connectionFactoryType));

    UseConnectionFactoryCore(connectionFactoryType);
    return this;
  }
}
