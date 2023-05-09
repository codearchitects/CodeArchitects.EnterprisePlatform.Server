using MySqlConnector;

namespace CodeArchitects.Platform.Data.AdoNet.MySQL;

/// <summary>
/// Extension methods for <see cref="MySQLProvider"/>.
/// </summary>
public static class MySQLProviderExtensions
{
  /// <summary>
  /// Specifies a factory that creates instances of <see cref="MySqlConnection"/>.
  /// </summary>
  /// <typeparam name="TConnectionFactory">The connection factory type.</typeparam>
  /// <param name="provider">The database provider.</param>
  /// <returns>The same <see cref="MySQLProvider"/> for further configuration.</returns>
  public static MySQLProvider UseConnectionFactory<TConnectionFactory>(this MySQLProvider provider)
    where TConnectionFactory : IConnectionFactory<MySqlConnection>
  {
    return provider.UseConnectionFactory(typeof(TConnectionFactory));
  }
}
