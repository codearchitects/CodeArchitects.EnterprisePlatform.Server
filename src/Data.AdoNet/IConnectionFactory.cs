using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

public interface IConnectionFactory<out TDbConnection>
  where TDbConnection : DbConnection
{
  TDbConnection CreateConnection();
}
