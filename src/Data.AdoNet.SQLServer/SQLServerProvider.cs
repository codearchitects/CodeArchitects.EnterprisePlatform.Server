using CodeArchitects.Platform.Data.AdoNet.SQLServer.Command;
using Microsoft.Data.SqlClient;

namespace CodeArchitects.Platform.Data.AdoNet.SQLServer;

/// <summary>
/// The SQL Server database provider.
/// </summary>
public class SQLServerProvider : DatabaseProvider
{
  private protected override Type DbConnectionType => typeof(SqlConnection);

  private protected override Type DbCommandType => typeof(SqlCommand);

  internal override Type SyntaxProviderType => typeof(SQLServerSyntaxProvider);

  /// <summary>
  /// Specifies the connection string to the database.
  /// </summary>
  /// <param name="connectionString">The connection string.</param>
  /// <returns>The same <see cref="SQLServerProvider"/> for further configuration.</returns>
  public SQLServerProvider UseConnection(string connectionString)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));

    DelegateConnectionFactory = new DelegateConnectionFactory<SqlConnection>(() => new SqlConnection(connectionString));
    return this;
  }

  /// <summary>
  /// Specifies the connection string to the database.
  /// </summary>
  /// <param name="connectionString">The connection string.</param>
  /// <param name="credential">The credentials to use for authentication.</param>
  /// <returns>The same <see cref="SQLServerProvider"/> for further configuration.</returns>
  public SQLServerProvider UseConnection(string connectionString, SqlCredential credential)
  {
    if (connectionString is null)
      throw new ArgumentNullException(nameof(connectionString));
    if (credential is null)
      throw new ArgumentNullException(nameof(credential));

    DelegateConnectionFactory = new DelegateConnectionFactory<SqlConnection>(() => new SqlConnection(connectionString, credential));
    return this;
  }

  /// <summary>
  /// Specifies a factory that creates instances of <see cref="SqlConnection"/>.
  /// </summary>
  /// <param name="connectionFactory">The factory function.</param>
  /// <returns>The same <see cref="SQLServerProvider"/> for further configuration.</returns>
  public SQLServerProvider UseConnectionFactory(Func<SqlConnection> connectionFactory)
  {
    if (connectionFactory is null)
      throw new ArgumentNullException(nameof(connectionFactory));

    DelegateConnectionFactory = new DelegateConnectionFactory<SqlConnection>(connectionFactory);
    return this;
  }

  /// <summary>
  /// Specifies a factory that creates instances of <see cref="SqlConnection"/>.
  /// </summary>
  /// <param name="connectionFactoryType">The connection factory type. It must implement <see cref="IConnectionFactory{TDbConnection}"/> of <see cref="SqlConnection"/>.</param>
  /// <returns>The same <see cref="SQLServerProvider"/> for further configuration.</returns>
  public SQLServerProvider UseConnectionFactory(Type connectionFactoryType)
  {
    if (connectionFactoryType is null)
      throw new ArgumentNullException(nameof(connectionFactoryType));

    UseConnectionFactoryCore(connectionFactoryType);
    return this;
  }
}
