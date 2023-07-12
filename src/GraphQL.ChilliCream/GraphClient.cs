using CodeArchitects.Platform.GraphQL.ChilliCream.Document;
using CodeArchitects.Platform.GraphQL.Document;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class GraphClient<TDocumentRoot> : GraphClient<Utf8Document, TDocumentRoot>
  where TDocumentRoot : class
{
  private readonly IOperationExecutorProvider _executorProvider;
  private readonly IVariableExtractorProvider _extractorProvider;

  public GraphClient(IDocumentCache<Utf8Document> documentCache, Func<GraphDocument, Utf8Document> compileDocument, IOperationExecutorProvider executorProvider, IVariableExtractorProvider extractorProvider)
    : base(documentCache, compileDocument)
  {
    _executorProvider = executorProvider;
    _extractorProvider = extractorProvider;
  }

  protected override IGraphRequest<TResult> CreateRequest<TResult>(Utf8Document utf8Document)
    where TResult : class
  {
    IOperationExecutor<TResult> executor = _executorProvider.GetExecutor<TResult>();

    return new GraphRequest<TResult>(executor, utf8Document);
  }

  protected override IGraphRequest<TResult, TVariables> CreateRequest<TResult, TVariables>(Utf8Document utf8Document)
    where TResult : class
  {
    IOperationExecutor<TResult> executor = _executorProvider.GetExecutor<TResult>();
    VariableExtractor<TVariables> extract = _extractorProvider.GetExtractor<TVariables>();

    return new GraphRequest<TResult, TVariables>(executor, utf8Document, extract);
  }
}
