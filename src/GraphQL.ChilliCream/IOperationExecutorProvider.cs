using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal interface IOperationExecutorProvider
{
  IOperationExecutor<TResult> GetExecutor<TResult>()
    where TResult : class;
}
