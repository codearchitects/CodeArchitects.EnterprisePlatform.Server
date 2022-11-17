using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal interface IStateManager<TDbConnection> : IStateManager
  where TDbConnection : DbConnection
{
  TDbConnection Connection { get; }

  void AddExecution(Execution<TDbConnection> execution);
}
