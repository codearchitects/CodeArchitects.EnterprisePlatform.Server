using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal interface IExecutor
{
  Task<TEntity?> ExecuteSelectCommandAsync<TEntity, TKey>(DbCommand command, TKey key, in NavigationSpec<TEntity, TKey> spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;
  
  Task ExecuteInsertCommandAsync<TEntity, TKey>(DbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteUpdateCommandAsync<TEntity, TKey>(DbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteUpsertCommandAsync<TEntity, TKey>(DbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteDeleteCommandAsync<TEntity, TKey>(DbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteDeleteCommandAsync<TEntity, TKey>(DbCommand command, TKey key, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  void VisitGraph<TState>(object node, IEntityModel model, TState state, VisitNodeCallback<TState> callback);

  Task VisitGraphAsync<TState>(object node, IEntityModel model, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken);
}
