using CodeArchitects.Platform.GraphQL.ChilliCream.Document;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class GraphRequest<TResult> : GraphRequestBase<TResult>, IGraphRequest<TResult>
  where TResult : class
{
  private readonly IGraphDocument _document;

  public GraphRequest(IOperationExecutor<TResult> executor, IGraphDocument document)
    : base(executor)
  {
    _document = document;
  }

  public Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default)
  {
    OperationRequest request = _document.CreateRequest(null, null, RequestStrategy.Default);
    return base.ExecuteAsync(request, cancellationToken);
  }
}