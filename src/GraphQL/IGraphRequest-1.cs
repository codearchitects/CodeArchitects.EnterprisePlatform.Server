namespace CodeArchitects.Platform.GraphQL;

public interface IGraphRequest<TResult>
{
  Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default);
}
