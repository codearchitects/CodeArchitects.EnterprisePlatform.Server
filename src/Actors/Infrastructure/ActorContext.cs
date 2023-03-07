using CodeArchitects.Platform.Actors.Bindings;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Common.Expressions;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal class ActorContext<TActor, TState> : IActorContext<TActor>, IActorManager<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  private readonly IActorDescriptor<TActor, TState> _descriptor;
  private readonly IActivityManager<TActor> _activityManager;
  private readonly IActorHost<TActor, TState> _host;
  private readonly List<IActorBinding<TActor>> _bindings;
  private ExecutionSection _section;

  public ActorContext(
    IServiceProvider services,
    IActorDescriptor<TActor, TState> descriptor,
    IActivityManager<TActor> activityManager,
    IActorHost<TActor, TState> host,
    TState state,
    int implementationId)
  {
    _descriptor = descriptor;
    _activityManager = activityManager;
    _host = host;
    State = state;
    _bindings = new();

    Actor = CreateInstance(services, descriptor, state, implementationId);
  }

  public TActor Actor { get; }

  public TState State { get; }

  public string ActorId => _host.ActorId;

  public int DefaultImplementationId => _descriptor.DefaultImplementation.Id;

  public JsonSerializerOptions JsonSerializerOptions => _descriptor.JsonSerializerOptions;

  public Type ActivityType => _descriptor.ActivityBaseType;

  public void Become<TImplementation>()
    where TImplementation : class, TActor
  {
    try
    {
      State.ImplementationId = _descriptor.GetImplementation(typeof(TImplementation)).Id;
    }
    catch (ArgumentException ex)
    {
      throw new TypeArgumentException("Invalid implementation type.", ex);
    }
  }

  public void Become(Type implementationType)
  {
    State.ImplementationId = _descriptor.GetImplementation(implementationType).Id;
  }

  public BindingId RegisterBinding(Func<IBindingBuilder<TActor>, IBindingResult> configure)
  {
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));
    if (_section is not ExecutionSection.Constructor)
      throw new InvalidOperationException("Bindings may be only registered inside the actor's constructor.");

    return RegisterBindingCore(new ActorBinding<TActor>(), configure);
  }

  public BindingId RegisterBinding<TImplementation>(Func<IBindingBuilder<TImplementation>, IBindingResult> configure)
    where TImplementation : class, TActor
  {
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));
    if (_section is not ExecutionSection.Constructor)
      throw new InvalidOperationException("Bindings may be only registered inside the actor's constructor.");

    return RegisterBindingCore(new ActorBinding<TActor, TImplementation>(), configure);
  }

  public void EnableBinding(BindingId id)
  {
    EnableBindingCore(id._index);
  }

  public void DisableBinding(BindingId id)
  {
    DisableBindingCore(id._index);
  }

  public Task ScheduleAsync(Expression<Func<TActor, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleCoreAsync(ScheduleId.New(), 0, activity, options, cancellationToken);
  }

  public Task ScheduleAsync(Expression<Action<TActor>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleCoreAsync(ScheduleId.New(), 0, activity, options, cancellationToken);
  }

  public Task ScheduleAsync<TImplementation>(Expression<Func<TImplementation, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    int implementationId = _descriptor.GetImplementation(typeof(TImplementation)).Id;
    return ScheduleCoreAsync(ScheduleId.New(), implementationId, activity, options, cancellationToken);
  }

  public Task ScheduleAsync<TImplementation>(Expression<Action<TImplementation>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    int implementationId = _descriptor.GetImplementation(typeof(TImplementation)).Id;
    return ScheduleCoreAsync(ScheduleId.New(), implementationId, activity, options, cancellationToken);
  }

  public Task ScheduleAsync(ScheduleId id, Expression<Func<TActor, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleCoreAsync(id, 0, activity, options, cancellationToken);
  }

  public Task ScheduleAsync(ScheduleId id, Expression<Action<TActor>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleCoreAsync(id, 0, activity, options, cancellationToken);
  }

  public Task ScheduleAsync<TImplementation>(ScheduleId id, Expression<Func<TImplementation, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    int implementationId = _descriptor.GetImplementation(typeof(TImplementation)).Id;
    return ScheduleCoreAsync(id, implementationId, activity, options, cancellationToken);
  }

  public Task ScheduleAsync<TImplementation>(ScheduleId id, Expression<Action<TImplementation>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    int implementationId = _descriptor.GetImplementation(typeof(TImplementation)).Id;
    return ScheduleCoreAsync(id, implementationId, activity, options, cancellationToken);
  }

  public Task ScheduleAsync(ActivitySpec activitySpec, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activitySpec.Name is null)
      throw new ArgumentException("The activity name was null.", nameof(activitySpec));
    if (activitySpec.Arguments is null)
      throw new ArgumentException("The activity argument list was null.", nameof(activitySpec));

    Activity<TActor> activity = CreateActivity(in activitySpec);
    return _host.ScheduleAsync(ScheduleId.New(), activity, options ?? SchedulingOptions.Now, cancellationToken);
  }

  public Task ScheduleAsync(ScheduleId id, ActivitySpec activitySpec, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activitySpec.Name is null)
      throw new ArgumentException("The activity name was null.", nameof(activitySpec));
    if (activitySpec.Arguments is null)
      throw new ArgumentException("The activity argument list was null.", nameof(activitySpec));

    Activity<TActor> activity = CreateActivity(in activitySpec);
    return _host.ScheduleAsync(id, activity, options ?? SchedulingOptions.Now, cancellationToken);
  }

  public Task UnscheduleAsync(ScheduleId id, CancellationToken cancellationToken = default)
  {
    return _host.UnscheduleAsync(id, cancellationToken);
  }

  private Activity<TActor> CreateActivity(in ActivitySpec activitySpec)
  {
    int implementationId = activitySpec.ImplementationType is { } implementationType
      ? _descriptor.GetImplementation(implementationType).Id
      : 0;

    return _activityManager.CreateActivity(implementationId, activitySpec.Name, activitySpec.Arguments);
  }

  private Task ScheduleCoreAsync(ScheduleId id, int implementationId, LambdaExpression activityExpression, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    ParameterExpression actorParam = activityExpression.Parameters[0];

    if (activityExpression.Body is not MethodCallExpression methodCallExpression || methodCallExpression.Object != actorParam)
      throw new ArgumentException($"The '{nameof(activityExpression)}' expression should represent a call to an instance method of the current actor.", nameof(activityExpression));

    DynamicArgumentList arguments = new(methodCallExpression.Arguments);
    Activity<TActor> activity = _activityManager.CreateActivity(implementationId, methodCallExpression.Method, arguments);

    return _host.ScheduleAsync(id, activity, options ?? SchedulingOptions.Now, cancellationToken);
  }

  public void OnActivityBegin()
  {
    VerifyBindingPreconditions();
    _section = ExecutionSection.Activity;
  }

  public void OnMethodBegin()
  {
    VerifyBindingPreconditions();
    _section = ExecutionSection.Method;
  }

  public void OnExecutionEnd()
  {
    _section = ExecutionSection.None;
    _descriptor.UpdateState(Actor, State);
  }

  public async Task ExecuteBindingsAsync(CancellationToken cancellationToken)
  {
    _section = ExecutionSection.Bindings;

    for (int index = 0; index < _bindings.Count; index++)
    {
      IActorBinding<TActor> binding = _bindings[index];

      if (!State.IsBindingEnabled(index))
        continue;

      await binding.ExecuteAsync(Actor, cancellationToken);
    }

    _section = ExecutionSection.None;
  }

  public BindingId RegisterBindingCore<TImplementation>(ActorBinding<TActor, TImplementation> binding, Func<IBindingBuilder<TImplementation>, IBindingResult> configure)
    where TImplementation : class, TActor
  {
    int index = _bindings.Count;
    if (index >= 32)
      throw new InvalidOperationException("Exceeded the maximum number of bindings.");

    configure(binding);
    _bindings.Add(binding);
    
    if (binding.IsEnabled)
    {
      EnableBindingCore(index);
    }
    else
    {
      DisableBindingCore(index);
    }

    return new BindingId(index);
  }

  private TActor CreateInstance(IServiceProvider services, IActorDescriptor<TActor, TState> descriptor, TState state, int implementationId)
  {
    _section = ExecutionSection.Constructor;
    TActor instance = descriptor.CreateInstance(implementationId, services, state, this);
    _section = ExecutionSection.None;

    return instance;
  }

  private void VerifyBindingPreconditions()
  {
    foreach (IActorBinding<TActor> binding in _bindings)
    {
      binding.VerifyPreCondition(Actor);
    }
  }

  private void EnableBindingCore(int index)
  {
    State.EnabledBindings |= (1 << index);
  }

  private void DisableBindingCore(int index)
  {
    State.EnabledBindings &= ~(1 << index);
  }

  private class DynamicArgumentList : IReadOnlyList<object?>
  {
    private readonly ReadOnlyCollection<Expression> _arguments;

    public DynamicArgumentList(ReadOnlyCollection<Expression> arguments)
    {
      _arguments = arguments;
    }

    public object? this[int index] => ExpressionEvaluator.Evaluate(_arguments[index]);

    public int Count => _arguments.Count;

    public IEnumerator<object?> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
  }
}
