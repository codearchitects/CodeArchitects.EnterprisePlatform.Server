using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

public delegate Task Execution<in TDbConnection>(TDbConnection connection, CancellationToken cancellationToken)
  where TDbConnection : class, IDbConnection;
