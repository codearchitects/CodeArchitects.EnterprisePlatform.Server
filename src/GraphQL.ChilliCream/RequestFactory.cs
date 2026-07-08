using CodeArchitects.Platform.GraphQL.ChilliCream.Document;
using CodeArchitects.Platform.GraphQL.Document;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class RequestFactory : IRequestFactory<Utf8Document>
{
  private readonly IOperationExecutorProvider _executorProvider;
  private readonly IVariableExtractorProvider _extractorProvider;

  public RequestFactory(IOperationExecutorProvider executorProvider, IVariableExtractorProvider extractorProvider)
  {
    _executorProvider = executorProvider;
    _extractorProvider = extractorProvider;
  }

  public IGraphRequest<TResult> CreateRequest<TResult>(OperationType operationType, string name, Utf8Document utf8Document)
    where TResult : class
  {
    if (operationType is not OperationType.Query and not OperationType.Mutation)
      throw new ArgumentException($"Operation of type '{operationType}' is not supported.", nameof(operationType));

    IOperationExecutor<TResult> executor = _executorProvider.GetExecutor<TResult>();

    return new GraphRequest<TResult>(executor, utf8Document);
  }

  public IGraphRequest<TResult, TVariables> CreateRequest<TResult, TVariables>(OperationType operationType, string name, Utf8Document utf8Document)
    where TResult : class
    where TVariables : notnull
  {
    if (operationType is not OperationType.Query and not OperationType.Mutation)
      throw new ArgumentException($"Operation of type '{operationType}' is not supported.", nameof(operationType));

    IOperationExecutor<TResult> executor = _executorProvider.GetExecutor<TResult>();
    VariableExtractor<TVariables> extract = _extractorProvider.GetExtractor<TVariables>();

    return new GraphRequest<TResult, TVariables>(executor, utf8Document, extract);
  }
}
