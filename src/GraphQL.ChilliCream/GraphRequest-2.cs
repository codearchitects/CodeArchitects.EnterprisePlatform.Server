using CodeArchitects.Platform.GraphQL.ChilliCream.Document;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class GraphRequest<TResult, TVariables> : GraphRequestBase<TResult>, IGraphRequest<TResult, TVariables>
  where TResult : class
  where TVariables : notnull
{
  private readonly IGraphDocument<TResult, TVariables> _document;
  private readonly VariableExtractor<TVariables> _extract;

  public GraphRequest(IOperationExecutor<TResult> executor, IGraphDocument<TResult, TVariables> document, VariableExtractor<TVariables> extract)
    : base(executor)
  {
    _document = document;
    _extract = extract;
  }

  public Task<TResult> ExecuteAsync(TVariables variables, CancellationToken cancellationToken = default)
  {
    (IReadOnlyDictionary<string, object?> variableDictionary, IReadOnlyDictionary<string, Upload?> fileDictionary) = _extract(variables);
    OperationRequest request = _document.CreateRequest(variableDictionary, fileDictionary, RequestStrategy.Default);

    return ExecuteAsync(request, cancellationToken);
  }
}
