namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal interface IExecutorDictionaryFactory
{
  IReadOnlyDictionary<Type, object> CreateExecutors(IExecutor executor);
}
