using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal class DelegateConnectionFactory<TDbConnection> : IConnectionFactory<TDbConnection>
  where TDbConnection : DbConnection
{
  private readonly Func<TDbConnection> _factory;

  public DelegateConnectionFactory(Func<TDbConnection> factory)
  {
    _factory = factory;
  }

  public TDbConnection CreateConnection()
  {
    return _factory();
  }
}
