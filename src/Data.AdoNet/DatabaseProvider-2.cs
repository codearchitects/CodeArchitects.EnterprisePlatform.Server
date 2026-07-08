using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.Features.Concurrency;
using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

/// <summary>
/// Represents a database provider that uses a specific <see cref="DbConnection"/> and <see cref="DbCommand"/>.
/// </summary>
/// <typeparam name="TDbConnection">The type of the database connection.</typeparam>
/// <typeparam name="TDbCommand">The type of the database command.</typeparam>
public abstract class DatabaseProvider<TDbConnection, TDbCommand> : DatabaseProvider
  where TDbConnection : DbConnection
  where TDbCommand : DbCommand
{
  private protected sealed override Type DbConnectionType => typeof(TDbConnection);

  private protected sealed override Type DbCommandType => typeof(TDbCommand);

  internal override Type DataContextType => typeof(DataContext<TDbConnection, TDbCommand>);

  internal sealed override void ApplySeed(IServiceProvider services, DataSeed seed)
  {
    IStateManager<TDbConnection> stateManager = (IStateManager<TDbConnection>)services.GetService(typeof(IStateManager<TDbConnection>))!;
    ICommandBuilder<TDbCommand> commandBuilder = (ICommandBuilder<TDbCommand>)services.GetService(typeof(ICommandBuilder<TDbCommand>))!;
    IDataModel dataModel = (IDataModel)services.GetService(typeof(IDataModel))!;

    Seeder<TDbConnection, TDbCommand> seeder = new(stateManager, commandBuilder, dataModel);
    seeder.Apply(seed);
  }

  internal sealed override object CreateCommandBuilder(ISqlTextBuilder sqlBuilder, IConcurrencyContext concurrencyContext)
  {
    return CreateCommandBuilderCore(sqlBuilder, concurrencyContext);
  }

  private protected virtual CommandBuilder<TDbCommand> CreateCommandBuilderCore(ISqlTextBuilder sqlBuilder, IConcurrencyContext concurrencyContext)
  {
    return new CommandBuilder<TDbCommand>(sqlBuilder, concurrencyContext);
  }
}
