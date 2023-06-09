namespace CodeArchitects.Platform.GraphQL.Document;

internal interface IDocumentRoot
{
  TField Field<TField>(string name);
}
