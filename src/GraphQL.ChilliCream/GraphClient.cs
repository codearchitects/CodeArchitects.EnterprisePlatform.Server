using CodeArchitects.Platform.GraphQL.ChilliCream.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class GraphClient<TDocumentRoot> : IGraphClient<TDocumentRoot>
  where TDocumentRoot : class
{
  private readonly IOperationExecutorProvider _executorProvider;
  private readonly IDocumentBuilder<TDocumentRoot> _documentBuilder;
  private readonly IVariableExtractorProvider _extractorProvider;

  public GraphClient(IOperationExecutorProvider executorProvider, IDocumentBuilder<TDocumentRoot> documentBuilder, IVariableExtractorProvider extractorProvider)
  {
    _executorProvider = executorProvider;
    _documentBuilder = documentBuilder;
    _extractorProvider = extractorProvider;
  }

  public IGraphRequest<TResult> Request<TResult>(GraphQL.Document.IGraphDocument<TResult> document)
    where TResult : class
  {
    if (document is null)
      throw new ArgumentNullException(nameof(document));

    if (document is not IGraphDocument<TResult> chilliCreamDocument)
      throw new ArgumentException($"'{nameof(document)}' should be a ChilliCream graph document.", nameof(document));

    return RequestCore(chilliCreamDocument);
  }

  public IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(GraphQL.Document.IGraphDocument<TResult, TVariables> document)
    where TResult : class
    where TVariables : notnull
  {
    if (document is null)
      throw new ArgumentNullException(nameof(document));

    if (document is not IGraphDocument<TResult, TVariables> chilliCreamDocument)
      throw new ArgumentException($"'{nameof(document)}' should be a ChilliCream graph document.", nameof(document));

    return RequestCore(chilliCreamDocument);
  }

  public IGraphRequest<TResult> Request<TResult>(Func<IDocumentBuilder<TDocumentRoot>, GraphQL.Document.IGraphDocument<TResult>> buildDocument)
    where TResult : class
  {
    if (buildDocument is null)
      throw new ArgumentNullException(nameof(buildDocument));

    GraphQL.Document.IGraphDocument<TResult> document = buildDocument(_documentBuilder);
    if (document is not IGraphDocument<TResult> chilliCreamDocument)
      throw new ArgumentException($"'{nameof(buildDocument)}' should return a ChilliCream graph document.", nameof(buildDocument));

    return RequestCore(chilliCreamDocument);
  }

  public IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(Func<IDocumentBuilder<TDocumentRoot>, GraphQL.Document.IGraphDocument<TResult, TVariables>> buildDocument)
    where TResult : class
    where TVariables : notnull
  {
    if (buildDocument is null)
      throw new ArgumentNullException(nameof(buildDocument));

    GraphQL.Document.IGraphDocument<TResult, TVariables> document = buildDocument(_documentBuilder);
    if (document is not IGraphDocument<TResult, TVariables> chilliCreamDocument)
      throw new ArgumentException($"'{nameof(buildDocument)}' should return a ChilliCream graph document.", nameof(buildDocument));

    return RequestCore(chilliCreamDocument);
  }

  private IGraphRequest<TResult> RequestCore<TResult>(IGraphDocument<TResult> document)
    where TResult : class
  {
    IOperationExecutor<TResult> executor = _executorProvider.GetExecutor<TResult>();

    return new GraphRequest<TResult>(executor, document);
  }

  private IGraphRequest<TResult, TVariables> RequestCore<TResult, TVariables>(IGraphDocument<TResult, TVariables> document)
    where TResult : class
    where TVariables : notnull
  {
    IOperationExecutor<TResult> executor = _executorProvider.GetExecutor<TResult>();
    VariableExtractor<TVariables> extractor = _extractorProvider.GetExtractor<TVariables>();

    return new GraphRequest<TResult, TVariables>(executor, document, extractor);
  }
}
