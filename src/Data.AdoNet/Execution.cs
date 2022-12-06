using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

public delegate Task Execution<in TDbConnection, in TDbTransaction>(TDbConnection connection, TDbTransaction? transaction, CancellationToken cancellationToken)
  where TDbConnection : class, IDbConnection
  where TDbTransaction : class, IDbTransaction;
