using CodeArchitects.Platform.GraphQL.Document;

namespace CodeArchitects.Platform.GraphQL;

internal interface IRequestFactory<TUtf8Document>
  where TUtf8Document : IUtf8Document
{
  IGraphRequest<TResult> CreateRequest<TResult>(OperationType operationType, string name, TUtf8Document utf8Document)
    where TResult : class;

  IGraphRequest<TResult, TVariables> CreateRequest<TResult, TVariables>(OperationType operationType, string name, TUtf8Document utf8Document)
    where TResult : class
    where TVariables : notnull;
}
