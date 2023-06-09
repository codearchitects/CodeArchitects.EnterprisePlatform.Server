namespace CodeArchitects.Platform.GraphQL;

internal interface IGraphRequest<TResult>
  where TResult : class
{
  Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default);
}
