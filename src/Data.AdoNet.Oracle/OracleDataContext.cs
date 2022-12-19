using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Model;
using Oracle.ManagedDataAccess.Client;

namespace CodeArchitects.Platform.Data.AdoNet.Oracle;

internal class OracleDataContext : DataContext<OracleConnection, OracleCommand>
{
  public OracleDataContext(
    IStateManager<OracleConnection> stateManager,
    IExecutor<OracleCommand> executor,
    ICommandInterceptor<OracleCommand> interceptor,
    IDataModel model)
    : base(stateManager, executor, interceptor, model)
  {
  }

  protected override OracleCommand CreateCommand(OracleConnection connection)
  {
    return connection.CreateCommand();
  }
}
