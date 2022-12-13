using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

public interface IDataContext<TDbConnection> : IDataContext
  where TDbConnection : DbConnection
{
  new TDbConnection Connection { get; }

  Task BatchExecuteAsync(Execution<TDbConnection, DbTransaction> execution, CancellationToken cancellationToken = default);
}
