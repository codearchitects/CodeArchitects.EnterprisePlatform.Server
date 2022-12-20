using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

/// <summary>
/// Represents an operation to be executed on a database connection and transaction.
/// </summary>
/// <typeparam name="TDbConnection">The type of the database connection.</typeparam>
/// <typeparam name="TDbTransaction">The type of the database transaction.</typeparam>
/// <param name="connection">The database connection.</param>
/// <param name="transaction">The database transaction, if it was created.</param>
/// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
public delegate Task Execution<in TDbConnection, in TDbTransaction>(TDbConnection connection, TDbTransaction? transaction, CancellationToken cancellationToken)
  where TDbConnection : class, IDbConnection
  where TDbTransaction : class, IDbTransaction;
