using CodeArchitects.Platform.Common.Utils;
using Microsoft.Extensions.Caching.Memory;

namespace CodeArchitects.Platform.GraphQL.Document;

internal class DocumentCache<TUtf8Document> : IDocumentCache<TUtf8Document>
  where TUtf8Document : IUtf8Document
{
  private readonly IMemoryCache _cache;
  private readonly Synchronizer _synchronizer;

  public DocumentCache(IMemoryCache cache, Synchronizer synchronizer)
  {
    _cache = cache;
    _synchronizer = synchronizer;
  }

  public TUtf8Document GetOrCompile<TDocument>(TDocument document, Func<TDocument, TUtf8Document> compile)
    where TDocument : class
  {
    if (_cache.TryGetValue(document, out TUtf8Document utf8Document))
      return utf8Document;

    using (_synchronizer.Sync(document))
    {
      if (_cache.TryGetValue(document, out utf8Document))
        return utf8Document;

      utf8Document = compile(document);

      using ICacheEntry entry = _cache.CreateEntry(document);
      entry.Value = utf8Document;
      entry.Size = utf8Document.Content.Length / 4;
    }

    return utf8Document;
  }
}
