using CodeArchitects.Platform.GraphQL.ChilliCream.Document;
using CodeArchitects.Platform.GraphQL.Document;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class ChilliCreamGraphClient<TDocumentRoot> : GraphClient<ChilliCreamUtf8Document, TDocumentRoot>
  where TDocumentRoot : class
{
  private readonly IOperationExecutorProvider _executorProvider;
  private readonly IVariableExtractorProvider _extractorProvider;

  public ChilliCreamGraphClient(IDocumentCache<ChilliCreamUtf8Document> documentCache, Func<GraphDocument, ChilliCreamUtf8Document> compileDocument, IOperationExecutorProvider executorProvider, IVariableExtractorProvider extractorProvider)
    : base(documentCache, compileDocument)
  {
    _executorProvider = executorProvider;
    _extractorProvider = extractorProvider;
  }

  protected override IGraphRequest<TResult> CreateRequest<TResult>(ChilliCreamUtf8Document utf8Document)
  {
    throw new NotImplementedException();
  }

  protected override IGraphRequest<TResult, TVariables> CreateRequest<TResult, TVariables>(ChilliCreamUtf8Document utf8Document)
  {
    throw new NotImplementedException();
  }
}
