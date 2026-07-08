namespace CodeArchitects.Platform.Actors.Bindings;

internal readonly struct BindingActivity<TActor>
  where TActor : class
{
  private readonly Action<TActor>? _activity;
  private readonly Func<TActor, CancellationToken, Task>? _asyncActivity;

  public BindingActivity(Action<TActor> activity)
  {
    _activity = activity;
  }

  public BindingActivity(Func<TActor, CancellationToken, Task> asyncActivity)
  {
    _asyncActivity = asyncActivity;
  }

  public Task ExecuteAsync(TActor actor, CancellationToken cancellationToken)
  {
    if (_activity is not null)
    {
      _activity(actor);
      return Task.CompletedTask;
    }

    if (_asyncActivity is not null)
    {
      return _asyncActivity(actor, cancellationToken);
    }

    return Task.CompletedTask;
  }

  public static implicit operator BindingActivity<TActor>(Action<TActor> activity) => new(activity);

  public static implicit operator BindingActivity<TActor>(Func<TActor, CancellationToken, Task> asyncActivity) => new(asyncActivity);
}
