using CodeArchitects.Platform.CodeAnalysis;
using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

public interface IDataContext : Data.IDataContext
{
  IDbConnection Connection { get; }

  Task BatchExecuteAsync(Execution<IDbConnection, IDbTransaction> execution, bool startTransaction, CancellationToken cancellationToken = default);

  [Experimental]
  void VisitGraph<TEntity, TState>(TEntity entity, TState state, VisitNodeCallback<TState> callback)
    where TEntity : class;

  [Experimental]
  Task VisitGraphAsync<TEntity, TState>(TEntity entity, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken)
    where TEntity : class;
}
