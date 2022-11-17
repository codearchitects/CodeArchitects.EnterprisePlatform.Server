using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data;

internal abstract class StateManager : IStateManager, IUnitOfWorkManager
{
  private UnitOfWork? _current;

  [MemberNotNullWhen(true, nameof(_current))]
  protected bool IsActive => _current is not null;

  public IUnitOfWork Begin()
  {
    if (_current is not null)
      throw new InvalidOperationException("Another unit of work began and was not disposed.");

    _current = new UnitOfWork(this);
    return _current;
  }

  public Task SaveAsync(CancellationToken cancellationToken)
  {
    return _current is null
      ? SaveCoreAsync(cancellationToken)
      : Task.CompletedTask;
  }

  protected abstract Task SaveCoreAsync(CancellationToken cancellationToken);

  private sealed class UnitOfWork : IUnitOfWork
  {
    private readonly StateManager _manager;

    public UnitOfWork(StateManager manager) => _manager = manager;

    Task IUnitOfWork.CommitAsync(CancellationToken cancellationToken) => _manager.SaveCoreAsync(cancellationToken);

    void IDisposable.Dispose() => _manager._current = null;
  }
}
