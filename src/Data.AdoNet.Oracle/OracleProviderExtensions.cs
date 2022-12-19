using Oracle.ManagedDataAccess.Client;

namespace CodeArchitects.Platform.Data.AdoNet.Oracle;

public static class OracleProviderExtensions
{
  public static OracleProvider UseConnectionFactory<TConnectionFactory>(this OracleProvider provider)
    where TConnectionFactory : IConnectionFactory<OracleConnection>
  {
    return provider.UseConnectionFactory(typeof(TConnectionFactory));
  }
}
