namespace CodeArchitects.Platform.Data;

public interface IUnitOfWork : IAsyncDisposable
{
  Task SaveAsync(CancellationToken cancellationToken = default);

  // TODO: Add transactions
}
