namespace CodeArchitects.Platform.Data;

internal abstract class StateManager : IStateManager, IUnitOfWorkManager
{
  private UnitOfWork? _current;

  public IUnitOfWork Begin(CancellationToken cancellationToken = default)
  {
    return BeginCore(false, cancellationToken);
  }

  public IUnitOfWork Begin(bool autoSave, CancellationToken cancellationToken = default)
  {
    return BeginCore(autoSave, cancellationToken);
  }

  private IUnitOfWork BeginCore(bool autoSave, CancellationToken cancellationToken)
  {
    if (_current is not null)
      throw new InvalidOperationException("Another unit of work began and was not disposed.");

    _current = new UnitOfWork(this, autoSave, cancellationToken);
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
    private readonly bool _autoSave;
    private readonly CancellationToken _cancellationToken;
    private bool _isDisposed;

    public UnitOfWork(StateManager manager, bool autoSave, CancellationToken cancellationToken)
    {
      _manager = manager;
      _autoSave = autoSave;
      _cancellationToken = cancellationToken;
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
      return _manager.SaveCoreAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
      if (_isDisposed)
        return new(Task.CompletedTask);

      _isDisposed = true;
      _manager._current = null;
      return new(_autoSave ? _manager.SaveCoreAsync(_cancellationToken) : Task.CompletedTask);
    }
  }
}
