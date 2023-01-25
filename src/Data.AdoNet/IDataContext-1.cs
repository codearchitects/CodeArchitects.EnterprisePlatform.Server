using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

/// <summary>
/// Represents a <see cref="Data.IDataContext"/> that is based on ADO.NET and uses a specific type of database connection.
/// </summary>
/// <typeparam name="TDbConnection">The type of database connection used by this data context.</typeparam>
public interface IDataContext<TDbConnection> : IDataContext
  where TDbConnection : DbConnection
{
  /// <summary>
  /// The connection used by this data context.
  /// </summary>
  new TDbConnection Connection { get; }

  /// <summary>
  /// Executes an operation within the scope of a unit of work. Delays the execution of the specified operation until the unit of work completes.
  /// </summary>
  /// <param name="execution">The operation that will be executed on the unit of work completion.</param>
  /// <param name="startTransaction">A value indicating whether a new transaction should be started before executing the delayed operations.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task BatchExecuteAsync(Execution<TDbConnection, DbTransaction> execution, bool startTransaction, CancellationToken cancellationToken = default);
}
