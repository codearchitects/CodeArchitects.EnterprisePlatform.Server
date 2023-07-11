using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document;

public interface IGraphDocument<TResult> : GraphQL.Document.IGraphDocument<TResult>
{
  OperationRequest CreateRequest(RequestStrategy strategy);
}

public interface IGraphDocument<TResult, TVariables> : GraphQL.Document.IGraphDocument<TResult, TVariables>
  where TVariables : notnull
{
  OperationRequest CreateRequest(IReadOnlyDictionary<string, object?> variables, IReadOnlyDictionary<string, Upload?> files, RequestStrategy strategy);
}
