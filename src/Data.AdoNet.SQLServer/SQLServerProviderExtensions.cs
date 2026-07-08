using Microsoft.Data.SqlClient;

namespace CodeArchitects.Platform.Data.AdoNet.SQLServer;

/// <summary>
/// Extension methods for <see cref="SQLServerProvider"/>.
/// </summary>
public static class SQLServerProviderExtensions
{
  /// <summary>
  /// Specifies a factory that creates instances of <see cref="SqlConnection"/>.
  /// </summary>
  /// <typeparam name="TConnectionFactory">The connection factory type.</typeparam>
  /// <param name="provider">The database provider.</param>
  /// <returns>The same <see cref="SQLServerProvider"/> for further configuration.</returns>
  public static SQLServerProvider UseConnectionFactory<TConnectionFactory>(this SQLServerProvider provider)
    where TConnectionFactory : IConnectionFactory<SqlConnection>
  {
    return provider.UseConnectionFactory(typeof(TConnectionFactory));
  }
}
