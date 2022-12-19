using Npgsql;

namespace CodeArchitects.Platform.Data.AdoNet.PostgreSQL;

public static class PostgreSQLProviderExtensions
{
  public static PostgreSQLProvider UseConnectionFactory<TConnectionFactory>(this PostgreSQLProvider provider)
    where TConnectionFactory : IConnectionFactory<NpgsqlConnection>
  {
    return provider.UseConnectionFactory(typeof(TConnectionFactory));
  }
}
