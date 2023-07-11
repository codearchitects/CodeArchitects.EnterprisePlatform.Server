using CodeArchitects.Platform.GraphQL.ChilliCream.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal interface IGraphClient<TDocumentRoot>
  where TDocumentRoot : class
{
  IGraphRequest<TResult> Request<TResult>(IGraphDocument<TResult> document)
    where TResult : class;

  IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(IGraphDocument<TResult, TVariables> document)
    where TResult : class
    where TVariables : notnull;

  IGraphRequest<TResult> Request<TResult>(Func<IDocumentBuilder<TDocumentRoot>, GraphQL.Document.IGraphDocument<TResult>> buildDocument)
    where TResult : class;

  IGraphRequest<TResult, TVariables> Request<TResult, TVariables>(Func<IDocumentBuilder<TDocumentRoot>, GraphQL.Document.IGraphDocument<TResult, TVariables>> buildDocument)
    where TResult : class
    where TVariables : notnull;
}
