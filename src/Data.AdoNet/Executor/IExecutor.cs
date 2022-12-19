using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal interface IExecutor<TDbCommand>
  where TDbCommand : IDbCommand
{
  Task<TEntity?> ExecuteFindAsync<TEntity, TKey>(TDbCommand command, TKey key, in NavigationSpec<TEntity, TKey> spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;
  
  Task ExecuteInsertAsync<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteUpdateAsync<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteUpsertAsync<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteRemoveAsync<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteRemoveAsync<TEntity, TKey>(TDbCommand command, TKey key, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  void VisitGraph<TState>(object node, IEntityModel model, TState state, VisitNodeCallback<TState> callback);

  Task VisitGraphAsync<TState>(object node, IEntityModel model, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken);
}
