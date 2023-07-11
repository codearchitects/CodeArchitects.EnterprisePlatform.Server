namespace CodeArchitects.Platform.GraphQL;

internal interface IGraphRequest<TResult>
{
  Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default);
}
