using Npgsql;

namespace CodeArchitects.Platform.Data.AdoNet.PostgreSQL;

/// <summary>
/// Extension methods for <see cref="PostgreSQLProvider"/>.
/// </summary>
public static class PostgreSQLProviderExtensions
{
  /// <summary>
  /// Specifies a factory that creates instances of <see cref="NpgsqlConnection"/>.
  /// </summary>
  /// <typeparam name="TConnectionFactory">The connection factory type.</typeparam>
  /// <param name="provider">The Oracle provider.</param>
  /// <returns>The same <see cref="PostgreSQLProvider"/> for further configuration.</returns>
  public static PostgreSQLProvider UseConnectionFactory<TConnectionFactory>(this PostgreSQLProvider provider)
    where TConnectionFactory : IConnectionFactory<NpgsqlConnection>
  {
    return provider.UseConnectionFactory(typeof(TConnectionFactory));
  }
}
