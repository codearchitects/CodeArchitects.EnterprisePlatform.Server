using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal class Executor : IExecutor
{
  private readonly IReadOnlyDictionary<Type, object> _executors;

  public Executor(IExecutorDictionaryFactory executorDictionaryFactory)
  {
    _executors = executorDictionaryFactory.CreateExecutors(this);
  }

  public Task<TEntity?> ExecuteSelectCommandAsync<TEntity, TKey>(DbCommand command, TKey key, IReadOnlyCollection<string> paths, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return GetExecutor<TEntity, TKey>().ExecuteSelectCommandAsync(command, key, paths, cancellationToken);
  }

  private IExecutor<TEntity, TKey> GetExecutor<TEntity, TKey>()
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return (IExecutor<TEntity, TKey>)_executors[typeof(TEntity)];
  }
}
