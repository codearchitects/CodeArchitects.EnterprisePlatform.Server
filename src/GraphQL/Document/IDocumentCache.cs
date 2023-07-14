namespace CodeArchitects.Platform.GraphQL.Document;

internal interface IDocumentCache<TUtf8Document>
  where TUtf8Document : IUtf8Document
{
  TUtf8Document GetOrCompile(GraphDocument document, Func<GraphDocument, TUtf8Document> compile);
}