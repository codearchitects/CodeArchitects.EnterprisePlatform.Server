namespace CodeArchitects.Platform.GraphQL.Document;

internal interface IDocumentCache<TUtf8Document>
  where TUtf8Document : IUtf8Document
{
  TUtf8Document GetOrCompile<TDocument>(TDocument document, Func<TDocument, TUtf8Document> compile)
    where TDocument : class;
}