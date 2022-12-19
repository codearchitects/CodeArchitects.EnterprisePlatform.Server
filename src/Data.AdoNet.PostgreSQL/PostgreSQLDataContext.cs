using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Model;
using Npgsql;

namespace CodeArchitects.Platform.Data.AdoNet.PostgreSQL;

internal class PostgreSQLDataContext : DataContext<NpgsqlConnection, NpgsqlCommand>
{
  public PostgreSQLDataContext(
    IStateManager<NpgsqlConnection> stateManager,
    IExecutor<NpgsqlCommand> executor,
    ICommandInterceptor<NpgsqlCommand> interceptor,
    IDataModel model)
    : base(stateManager, executor, interceptor, model)
  {
  }

  protected override NpgsqlCommand CreateCommand(NpgsqlConnection connection)
  {
    return connection.CreateCommand();
  }
}
