using System.Collections.Concurrent;

namespace CodeArchitects.Platform.GraphQL.Document;

internal class DocumentCache<TUtf8Document> : IDocumentCache<TUtf8Document>
  where TUtf8Document : IUtf8Document
{
  private readonly ConcurrentDictionary<GraphDocument, TUtf8Document> _documents;

  public DocumentCache()
  {
    _documents = new();
  }

  public TUtf8Document GetOrCompile(GraphDocument document, Func<GraphDocument, TUtf8Document> compile)
  {
    return _documents.GetOrAdd(document, compile);
  }
}
