using CodeArchitects.Platform.Data.AdoNet.Command;
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

  private protected sealed override object CreateCommandBuilderCore(ISqlTextBuilder sqlBuilder)
  {
    return CreateCommandBuilder(sqlBuilder);
  }

  private protected virtual CommandBuilder<TDbCommand> CreateCommandBuilder(ISqlTextBuilder sqlBuilder)
  {
    return new CommandBuilder<TDbCommand>(sqlBuilder);
  }
}
