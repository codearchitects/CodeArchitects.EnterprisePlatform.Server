using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

public interface IAdoNetContext<TDbConnection> : IAdoNetContext
  where TDbConnection : DbConnection
{
  new TDbConnection Connection { get; }

  Task BatchExecuteAsync(Execution<TDbConnection, DbTransaction> execution, CancellationToken cancellationToken = default);
}
