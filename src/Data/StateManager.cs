namespace CodeArchitects.Platform.Data;

internal abstract class StateManager : IStateManager, IUnitOfWorkManager
{
  private UnitOfWork? _current;

  public IUnitOfWork Begin(bool autoSave = false, CancellationToken cancellationToken = default)
  {
    if (_current is not null)
      throw new InvalidOperationException("Another unit of work began and was not disposed.");

    _current = autoSave
      ? new AutoSaveUnitOfWork(this, cancellationToken)
      : new UnitOfWork(this);

    return _current;
  }

  public Task SaveAsync(CancellationToken cancellationToken)
  {
    return _current is null
      ? SaveCoreAsync(cancellationToken)
      : Task.CompletedTask;
  }

  protected abstract Task SaveCoreAsync(CancellationToken cancellationToken);

  private class UnitOfWork : IUnitOfWork
  {
    protected readonly StateManager _manager;

    public UnitOfWork(StateManager manager)
    {
      _manager = manager;
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
      return _manager.SaveCoreAsync(cancellationToken);
    }

    public virtual ValueTask DisposeAsync()
    {
      _manager._current = null;
      return new(Task.CompletedTask);
    }
  }

  private sealed class AutoSaveUnitOfWork : UnitOfWork
  {
    private readonly CancellationToken _cancellationToken;

    public AutoSaveUnitOfWork(StateManager manager, CancellationToken cancellationToken)
      : base(manager)
    {
      _cancellationToken = cancellationToken;
    }

    public override ValueTask DisposeAsync()
    {
      _manager._current = null;
      return new(_manager.SaveCoreAsync(_cancellationToken));
    }
  }
}
