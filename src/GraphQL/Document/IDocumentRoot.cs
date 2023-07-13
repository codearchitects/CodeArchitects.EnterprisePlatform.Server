namespace CodeArchitects.Platform.GraphQL.Document;

public interface IDocumentRoot
{
  TField Field<TField>(string name);
}
