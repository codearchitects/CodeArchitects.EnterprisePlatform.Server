using Microsoft.Data.SqlClient;

namespace CodeArchitects.Platform.Data.AdoNet.SQLServer;

public static class SQLServerProviderExtensions
{
  public static SQLServerProvider UseConnectionFactory<TConnectionFactory>(this SQLServerProvider provider)
    where TConnectionFactory : IConnectionFactory<SqlConnection>
  {
    return provider.UseConnectionFactory(typeof(TConnectionFactory));
  }
}
