using CodeArchitects.Platform.GraphQL.ChilliCream.Document;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class RequestFactory<TDocumentRoot> : IRequestFactory<Utf8Document>
  where TDocumentRoot : class
{
  private readonly IOperationExecutorProvider _executorProvider;
  private readonly IVariableExtractorProvider _extractorProvider;

  public RequestFactory(IOperationExecutorProvider executorProvider, IVariableExtractorProvider extractorProvider)
  {
    _executorProvider = executorProvider;
    _extractorProvider = extractorProvider;
  }

  public IGraphRequest<TResult> CreateRequest<TResult>(Utf8Document utf8Document)
    where TResult : class
  {
    IOperationExecutor<TResult> executor = _executorProvider.GetExecutor<TResult>();

    return new GraphRequest<TResult>(executor, utf8Document);
  }

  public IGraphRequest<TResult, TVariables> CreateRequest<TResult, TVariables>(Utf8Document utf8Document)
    where TResult : class
    where TVariables : notnull
  {
    IOperationExecutor<TResult> executor = _executorProvider.GetExecutor<TResult>();
    VariableExtractor<TVariables> extract = _extractorProvider.GetExtractor<TVariables>();

    return new GraphRequest<TResult, TVariables>(executor, utf8Document, extract);
  }
}
