namespace CodeArchitects.Platform.GraphQL.Document;

internal interface IGraphDocument<TResult>
{
}

internal interface IGraphDocument<TResult, TVariables>
  where TVariables : notnull
{
}
