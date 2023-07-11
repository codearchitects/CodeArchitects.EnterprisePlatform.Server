using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;

namespace CodeArchitects.Platform.GraphQL;

internal abstract class GraphClient<TDocumentRoot> : IGraphClient<TDocumentRoot>
  where TDocumentRoot : class
{
  private readonly IDocumentBuilder<TDocumentRoot> _documentBuilder;

  public GraphClient(IDocumentBuilder<TDocumentRoot> documentBuilder)
  {
    _documentBuilder = documentBuilder;
  }

  public IGraphRequest<TResult> Request<TResult>(IGraphDocument<TResult> document)
  {
    if (document is null)
      throw new ArgumentNullException(nameof(document));

    return RequestCore(document);
  }

  public IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(IGraphDocument<TResult, TVariables> document)
    where TVariables : notnull
  {
    if (document is null)
      throw new ArgumentNullException(nameof(document));

    return RequestCore(document);
  }

  public IGraphRequest<TResult> Request<TResult>(Func<IDocumentBuilder<TDocumentRoot>, IGraphDocument<TResult>> buildDocument)
  {
    if (buildDocument is null)
      throw new ArgumentNullException(nameof(buildDocument));

    IGraphDocument<TResult> document = buildDocument(_documentBuilder);
    return RequestCore(document);
  }

  public IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(Func<IDocumentBuilder<TDocumentRoot>, GraphDocument<TResult, TVariables>> buildDocument)
    where TVariables : notnull
  {
    if (buildDocument is null)
      throw new ArgumentNullException(nameof(buildDocument));

    IGraphDocument<TResult, TVariables> document = buildDocument(_documentBuilder);
    return RequestCore(document);
  }

  protected abstract IGraphRequest<TResult> RequestCore<TResult>(IGraphDocument<TResult> document);

  protected abstract IGraphRequest<TResult, TVariables> RequestCore<TResult, TVariables>(IGraphDocument<TResult, TVariables> document)
    where TVariables : notnull;
}
