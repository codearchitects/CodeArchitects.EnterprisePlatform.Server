namespace CodeArchitects.Platform.GraphQL;

internal interface IGraphRequest<TResult, TVariables>
  where TResult : class
  where TVariables : class
{
  Task<TResult> ExecuteAsync(TVariables variables, CancellationToken cancellationToken = default);
}
