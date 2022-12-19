using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Model;
using Microsoft.Data.SqlClient;

namespace CodeArchitects.Platform.Data.AdoNet.SQLServer;

internal class SQLServerDataContext : DataContext<SqlConnection, SqlCommand>
{
  public SQLServerDataContext(
    IStateManager<SqlConnection> stateManager,
    IExecutor<SqlCommand> executor,
    ICommandInterceptor<SqlCommand> interceptor,
    IDataModel model)
    : base(stateManager, executor, interceptor, model)
  {
  }

  protected override SqlCommand CreateCommand(SqlConnection connection)
  {
    return connection.CreateCommand();
  }
}
