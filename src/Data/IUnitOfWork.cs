namespace CodeArchitects.Platform.Data;

public interface IUnitOfWork : IDisposable
{
  Task CommitAsync(CancellationToken cancellationToken = default);
}
