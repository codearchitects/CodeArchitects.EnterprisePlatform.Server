using MySqlConnector;

namespace CodeArchitects.Platform.Data.AdoNet.MariaDB;

public static class MariaDBProviderExtensions
{
  public static MariaDBProvider UseConnectionFactory<TConnectionFactory>(this MariaDBProvider provider)
    where TConnectionFactory : IConnectionFactory<MySqlConnection>
  {
    return provider.UseConnectionFactory(typeof(TConnectionFactory));
  }
}
