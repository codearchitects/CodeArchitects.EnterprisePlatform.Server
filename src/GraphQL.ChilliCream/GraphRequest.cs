namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class GraphRequest<TResult, TVariables> : IGraphRequest<TResult>, IGraphRequest<TResult, TVariables>
  where TVariables : notnull
{
  public Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<TResult> ExecuteAsync(TVariables variables, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
