using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;

namespace CodeArchitects.Platform.GraphQL;

internal abstract class GraphClient<TUtf8Document, TDocumentRoot> : IGraphClient<TDocumentRoot>
  where TUtf8Document : IUtf8Document
  where TDocumentRoot : class
{
  private readonly IDocumentCache<TUtf8Document> _documentCache;
  private readonly Func<GraphDocument, TUtf8Document> _compileDocument;

  protected GraphClient(IDocumentCache<TUtf8Document> documentCache, Func<GraphDocument, TUtf8Document> compileDocument)
  {
    _documentCache = documentCache;
    _compileDocument = compileDocument;
  }

  protected abstract IGraphRequest<TResult> CreateRequest<TResult>(TUtf8Document utf8Document)
    where TResult : class;

  protected abstract IGraphRequest<TResult, TVariables> CreateRequest<TResult, TVariables>(TUtf8Document utf8Document)
    where TResult : class
    where TVariables : notnull;

  public IGraphRequest<TResult> Request<TResult>(GraphDocument<TResult> document)
    where TResult : class
  {
    if (document is null)
      throw new ArgumentNullException(nameof(document));

    TUtf8Document utf8Document = _documentCache.GetOrCompile(document, _compileDocument);

    return CreateRequest<TResult>(utf8Document);
  }

  public IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(GraphDocument<TResult, TVariables> document)
    where TResult : class
    where TVariables : notnull
  {
    if (document is null)
      throw new ArgumentNullException(nameof(document));
    
    TUtf8Document utf8Document = _documentCache.GetOrCompile(document, _compileDocument);

    return CreateRequest<TResult, TVariables>(utf8Document);
  }

  public IGraphRequest<TResult> Request<TResult>(Func<IDocumentBuilder<TDocumentRoot>, GraphDocument<TResult>> buildDocument)
    where TResult : class
  {
    if (buildDocument is null)
      throw new ArgumentNullException(nameof(buildDocument));

    GraphDocument<TResult> document = buildDocument(DocumentBuilder<TDocumentRoot>.Instance);
    TUtf8Document utf8Document = _compileDocument(document);

    return CreateRequest<TResult>(utf8Document);
  }

  public IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(Func<IDocumentBuilder<TDocumentRoot>, GraphDocument<TResult, TVariables>> buildDocument)
    where TResult : class
    where TVariables : notnull
  {
    if (buildDocument is null)
      throw new ArgumentNullException(nameof(buildDocument));

    GraphDocument<TResult, TVariables> document = buildDocument(DocumentBuilder<TDocumentRoot>.Instance);
    TUtf8Document utf8Document = _compileDocument(document);

    return CreateRequest<TResult, TVariables>(utf8Document);
  }

  protected static Func<GraphDocument, TUtf8Document> CreateGraphDocumentCompiler(IModel model, INodeContext nodeContext, IDocumentCompiler<TUtf8Document> documentCompiler)
  {
    return delegate (GraphDocument document)
    {
      IOperationDefinitionNode operationDefinition = document.CreateOperationDefinition(model, nodeContext);
      return documentCompiler.Compile(operationDefinition);
    };
  }
}
