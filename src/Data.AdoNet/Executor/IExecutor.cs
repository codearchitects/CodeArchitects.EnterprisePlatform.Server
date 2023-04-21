using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal interface IExecutor<TDbCommand>
  where TDbCommand : IDbCommand
{
  Task<TEntity?> ExecuteFindAsync<TEntity, TKey>(TDbCommand command, TKey key, INavigationRoot<TEntity, TKey> root, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteInsertAsync<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteInsertManyAsync<TEntity, TKey>(TDbCommand command, IEnumerable<TEntity> entities, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteUpdateAsync<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task ExecuteUpdateManyAsync<TEntity, TKey>(TDbCommand command, IEnumerable<TEntity> entities, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
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
}
