using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;

namespace CodeArchitects.Platform.GraphQL;

internal interface IGraphClient<TDocumentRoot>
  where TDocumentRoot : class
{
  IGraphRequest<TResult> Request<TResult>(IGraphDocument<TResult> document);

  IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(IGraphDocument<TResult, TVariables> document)
    where TVariables : notnull;

  IGraphRequest<TResult> Request<TResult>(Func<IDocumentBuilder<TDocumentRoot>, IGraphDocument<TResult>> buildDocument);

  IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(Func<IDocumentBuilder<TDocumentRoot>, GraphDocument<TResult, TVariables>> buildDocument)
    where TVariables : notnull;
}
