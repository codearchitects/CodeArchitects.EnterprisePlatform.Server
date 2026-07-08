using Microsoft.Extensions.DependencyInjection;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class OperationExecutorProvider : IOperationExecutorProvider
{
  private readonly IServiceProvider _services;

  public OperationExecutorProvider(IServiceProvider services)
  {
    _services = services;
  }

  public IOperationExecutor<TResult> GetExecutor<TResult>()
    where TResult : class
  {
    return _services.GetRequiredService<IOperationExecutor<TResult>>();
  }
}
