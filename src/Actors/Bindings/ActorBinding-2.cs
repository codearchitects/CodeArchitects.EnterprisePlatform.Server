namespace CodeArchitects.Platform.Actors.Bindings;

internal class ActorBinding<TActor, TImplementation> : IActorBinding<TActor>, IBindingBuilder<TImplementation>, IBindingResult
  where TActor : class
  where TImplementation : class, TActor
{
  private Predicate<TImplementation>? _preCondition;
  private Predicate<TImplementation>? _postCondition;
  private BindingActivity<TImplementation> _activity;
  private bool _preConditionMet;

  public bool IsEnabled { get; private set; }

  protected virtual TImplementation Cast(TActor actor)
  {
    return actor as TImplementation ?? throw new InvalidOperationException($"Actor is currently not of type '{typeof(TImplementation).Name}'.");
  }

  public void VerifyPreCondition(TActor actor)
  {
    TImplementation implementation = Cast(actor);

    if (_preCondition is null)
    {
      _preConditionMet = true;
      return;
    }

    _preConditionMet = _preCondition(implementation);
  }

  public Task ExecuteAsync(TActor actor, CancellationToken cancellationToken)
  {
    TImplementation implementation = Cast(actor);

    if (!_preConditionMet)
      return Task.CompletedTask;

    if (_postCondition is not null && !_postCondition(implementation))
      return Task.CompletedTask;

    return _activity.ExecuteAsync(implementation, cancellationToken);
  }

  IBindingBuilder<TImplementation> IBindingBuilder<TImplementation>.WithPreCondition(Predicate<TImplementation> preCondition)
  {
    _preCondition = preCondition;
    return this;
  }

  IBindingBuilder<TImplementation> IBindingBuilder<TImplementation>.WithPostCondition(Predicate<TImplementation> postCondition)
  {
    _postCondition = postCondition;
    return this;
  }

  IBindingBuilder<TImplementation> IBindingBuilder<TImplementation>.IsEnabled(bool enabled)
  {
    IsEnabled = enabled;
    return this;
  }

  IBindingResult IBindingBuilder<TImplementation>.BindTo(Func<TImplementation, CancellationToken, Task> activity)
  {
    _activity = activity;
    return this;
  }

  IBindingResult IBindingBuilder<TImplementation>.BindTo(Action<TImplementation> activity)
  {
    _activity = activity;
    return this;
  }
}
