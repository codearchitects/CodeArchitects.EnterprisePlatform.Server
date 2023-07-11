namespace CodeArchitects.Platform.GraphQL;

public interface IGraphRequest<TResult, TVariables>
  where TVariables : notnull
{
  Task<TResult> ExecuteAsync(TVariables variables, CancellationToken cancellationToken = default);
}
