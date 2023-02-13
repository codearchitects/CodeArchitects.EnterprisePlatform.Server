namespace CodeArchitects.Platform.Actors.Bindings;

internal class ActorBinding<TActor> : IBindingBuilder<TActor>, IBindingResult
  where TActor : class
{
  private Func<TActor, bool>? _preCondition;
  private Func<TActor, bool>? _postCondition;
  private Func<TActor, CancellationToken, Task>? _activity;
  private bool _preConditionMet;

  public bool IsEnabled { get; private set; }

  public void VerifyPreCondition(TActor actor)
  {
    if (_preCondition is null)
    {
      _preConditionMet = true;
      return;
    }

    _preConditionMet = _preCondition(actor);
  }

  public Task ExecuteAsync(TActor actor, CancellationToken cancellationToken)
  {
    if (!_preConditionMet || _activity is null)
      return Task.CompletedTask;

    if (_postCondition is not null && !_postCondition(actor))
      return Task.CompletedTask;

    return _activity.Invoke(actor, cancellationToken);
  }

  IBindingBuilder<TActor> IBindingBuilder<TActor>.WithPreCondition(Func<TActor, bool> preCondition)
  {
    _preCondition = preCondition;
    return this;
  }

  IBindingBuilder<TActor> IBindingBuilder<TActor>.WithPostCondition(Func<TActor, bool> postCondition)
  {
    _postCondition = postCondition;
    return this;
  }

  IBindingBuilder<TActor> IBindingBuilder<TActor>.IsEnabled(bool enabled)
  {
    IsEnabled = enabled;
    return this;
  }

  IBindingResult IBindingBuilder<TActor>.BindTo(Func<TActor, CancellationToken, Task> activity)
  {
    _activity = activity;
    return this;
  }
}
