using Oracle.ManagedDataAccess.Client;

namespace CodeArchitects.Platform.Data.AdoNet.Oracle;

/// <summary>
/// Extension methods for <see cref="OracleProvider"/>.
/// </summary>
public static class OracleProviderExtensions
{
  /// <summary>
  /// Specifies a factory that creates instances of <see cref="OracleConnection"/>.
  /// </summary>
  /// <typeparam name="TConnectionFactory">The connection factory type.</typeparam>
  /// <param name="provider">The database provider.</param>
  /// <returns>The same <see cref="OracleProvider"/> for further configuration.</returns>
  public static OracleProvider UseConnectionFactory<TConnectionFactory>(this OracleProvider provider)
    where TConnectionFactory : IConnectionFactory<OracleConnection>
  {
    return provider.UseConnectionFactory(typeof(TConnectionFactory));
  }
}
