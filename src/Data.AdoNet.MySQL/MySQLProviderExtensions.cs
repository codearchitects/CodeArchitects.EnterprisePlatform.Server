using MySqlConnector;

namespace CodeArchitects.Platform.Data.AdoNet.MySQL;

public static class MySQLProviderExtensions
{
  public static MySQLProvider UseConnectionFactory<TConnectionFactory>(this MySQLProvider provider)
    where TConnectionFactory : IConnectionFactory<MySqlConnection>
  {
    return provider.UseConnectionFactory(typeof(TConnectionFactory));
  }
}
