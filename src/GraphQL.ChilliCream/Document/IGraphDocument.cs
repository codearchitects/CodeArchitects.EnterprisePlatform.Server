using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document;

public interface IGraphDocument
{
  OperationRequest CreateRequest(IReadOnlyDictionary<string, object?>? variables, IReadOnlyDictionary<string, Upload?>? files, RequestStrategy strategy);
}

public interface IGraphDocument<TResult> : IGraphDocument, GraphQL.Document.IGraphDocument<TResult>
{
}

public interface IGraphDocument<TResult, TVariables> : IGraphDocument, GraphQL.Document.IGraphDocument<TResult, TVariables>
  where TVariables : notnull
{
}
