using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

public interface IAdoNetContext : IDataContext
{
  IDbConnection Connection { get; }

  Task BatchExecuteAsync(Execution<IDbConnection, IDbTransaction> execution, CancellationToken cancellationToken = default);

  void VisitGraph<TEntity, TState>(string entityName, TEntity entity, TState state, VisitNodeCallback<TState> callback)
    where TEntity : class;

  Task VisitGraphAsync<TEntity, TState>(string entityName, TEntity entity, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken)
    where TEntity : class;

  void VisitGraph<TEntity, TState>(TEntity entity, TState state, VisitNodeCallback<TState> callback)
    where TEntity : class;

  Task VisitGraphAsync<TEntity, TState>(TEntity entity, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken)
    where TEntity : class;
}
