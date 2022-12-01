using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal interface IExecutor
{
  Task<TEntity?> ExecuteSelectCommandAsync<TEntity, TKey>(DbConnection connection, TKey key, NavigationSpec<TEntity, TKey> spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;
  
  Task ExecuteInsertCommandAsync<TEntity, TKey>(DbConnection connection, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteUpdateCommandAsync<TEntity, TKey>(DbConnection connection, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteDeleteCommandAsync<TEntity, TKey>(DbConnection connection, TKey key, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  void VisitGraph<TState>(object node, IEntityModel model, TState state, VisitNodeCallback<TState> callback);

  Task VisitGraphAsync<TState>(object node, IEntityModel model, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken);
}
