using CodeArchitects.Platform.GraphQL.ChilliCream.Document;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class GraphRequest<TResult> : GraphRequestBase<TResult>, IGraphRequest<TResult>
  where TResult : class
{
  private readonly Utf8Document _document;

  public GraphRequest(IOperationExecutor<TResult> executor, Utf8Document document)
    : base(executor)
  {
    _document = document;
  }

  public Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default)
  {
    OperationRequest request = _document.CreateRequest(RequestStrategy.Default);
    return ExecuteAsync(request, cancellationToken);
  }
}