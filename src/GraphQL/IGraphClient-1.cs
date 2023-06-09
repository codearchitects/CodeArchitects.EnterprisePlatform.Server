using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;

namespace CodeArchitects.Platform.GraphQL;

internal interface IGraphClient<TDocumentRoot>
  where TDocumentRoot : class
{
  IGraphRequest<TResult> Request<TResult>(IGraphDocument<TResult> document)
    where TResult : class;

  IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(GraphDocument<TResult, TVariables> document)
    where TResult : class
    where TVariables : class;

  IGraphRequest<TResult> Request<TResult>(Func<IDocumentBuilder<TDocumentRoot>, IGraphDocument<TResult>> buildDocument)
    where TResult : class;

  IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(Func<IDocumentBuilder<TDocumentRoot>, GraphDocument<TResult, TVariables>> buildDocument)
    where TResult : class
    where TVariables : class;
}
