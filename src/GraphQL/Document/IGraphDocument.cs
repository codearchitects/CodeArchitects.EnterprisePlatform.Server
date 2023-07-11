namespace CodeArchitects.Platform.GraphQL.Document;

public interface IGraphDocument<TResult>
{
}

public interface IGraphDocument<TResult, TVariables>
  where TVariables : notnull
{
}
