using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class GraphRequestBase<TResult>
  where TResult : class
{
  private readonly IOperationExecutor<TResult> _executor;

  public GraphRequestBase(IOperationExecutor<TResult> executor)
  {
    _executor = executor;
  }

  protected async Task<TResult> ExecuteAsync(OperationRequest request, CancellationToken cancellationToken)
  {
    IOperationResult<TResult> result = await _executor.ExecuteAsync(request, cancellationToken);
    result.EnsureNoErrors();

    if (result.Data is null)
      throw new InvalidOperationException("Request returned no data.");

    return result.Data;
  }
}
