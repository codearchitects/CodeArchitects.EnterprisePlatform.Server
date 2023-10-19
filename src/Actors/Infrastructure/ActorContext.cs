using CodeArchitects.Platform.Actors.Bindings;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Common.Expressions;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal class ActorContext<TActor, TState> : IActorContext<TActor>, IActorManager<TActor>
  where TActor : class
  where TState : ActorState
{
  private readonly IServiceProvider _services;
  private readonly IActorDescriptor<TActor, TState> _descriptor;
  private readonly IActivityManager<TActor> _activityManager;
  private readonly IActorHost<TActor, TState> _host;

  private readonly List<IActorBinding<TActor>> _bindings;
  private ExecutionSection _section;
  private TActor? _actor;
  private TState? _state;

  public ActorContext(
    IServiceProvider services,
    IActorDescriptor<TActor, TState> descriptor,
    IActivityManager<TActor> activityManager,
    IActorHost<TActor, TState> host)
  {
    _services = services;
    _descriptor = descriptor;
    _activityManager = activityManager;
    _host = host;
    _bindings = new();
  }

  public TActor Actor
  {
    get
    {
      EnsureActorInitialized();
      return _actor;
    }
  }

  public string ActorId => _host.ActorId;

  public int DefaultImplementationId => _descriptor.DefaultImplementation.Id;

  public JsonSerializerOptions JsonSerializerOptions => _descriptor.JsonSerializerOptions;

  public Type ActivityType => _descriptor.ActivityBaseType;

  public void Become<TImplementation>()
    where TImplementation : class, TActor
  {
    EnsureStateInitialized();

    try
    {
      _state.ImplementationId = _descriptor.GetImplementation(typeof(TImplementation)).Id;
    }
    catch (ArgumentException ex)
    {
      throw new TypeArgumentException("Invalid implementation type.", ex);
    }
  }

  public void Become(Type implementationType)
  {
    EnsureStateInitialized();

    _state.ImplementationId = _descriptor.GetImplementation(implementationType).Id;
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
    EnsureStateInitialized();

    EnableBindingCore(_state, id._index);
  }

  public void DisableBinding(BindingId id)
  {
    EnsureStateInitialized();

    DisableBindingCore(_state, id._index);
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

  private BindingId RegisterBindingCore<TImplementation>(ActorBinding<TActor, TImplementation> binding, Func<IBindingBuilder<TImplementation>, IBindingResult> configure)
    where TImplementation : class, TActor
  {
    EnsureStateInitialized();

    int index = _bindings.Count;
    if (index >= 32)
      throw new InvalidOperationException("Exceeded the maximum number of bindings.");

    configure(binding);
    _bindings.Add(binding);
    
    if (binding.IsEnabled)
    {
      EnableBindingCore(_state, index);
    }
    else
    {
      DisableBindingCore(_state, index);
    }

    return new BindingId(index);
  }

  public async Task BeginMethodAsync(CancellationToken cancellationToken)
  {
    _state = await GetStateAsync(cancellationToken);

    InitActor(_state, _state.ImplementationId);
    VerifyBindingPreconditions(_actor);

    _section = ExecutionSection.Method;
  }

  public async Task BeginActivityAsync(Activity<TActor> activity, CancellationToken cancellationToken)
  {
    _state = await GetStateAsync(cancellationToken);

    int implementationId = activity.ImplementationId;
    if (implementationId == 0)
    {
      implementationId = _state.ImplementationId;
    }

    InitActor(_state, implementationId);
    VerifyBindingPreconditions(_actor);

    _section = ExecutionSection.Activity;
  }

  public async Task EndExecutionAsync(CancellationToken cancellationToken)
  {
    EnsureActorInitialized();
    EnsureStateInitialized();

    _section = ExecutionSection.None;
    await ExecuteBindingsAsync(cancellationToken);

    _descriptor.UpdateState(_actor, _state);

    await _host.SetStateAsync(_state, cancellationToken);
  }

  public void InitializeState(ActorState state)
  {
    if (_descriptor.IsPolymorphic)
    {
      state.ImplementationId = _descriptor.DefaultImplementation.Id;
    }
  }

  [MemberNotNull(nameof(_actor))]
  private void InitActor(TState state, int implementationId)
  {
    _section = ExecutionSection.Constructor;
    _actor = _descriptor.CreateInstance(implementationId, _services, state, this);
    _section = ExecutionSection.None;
  }

  private async Task<TState> GetStateAsync(CancellationToken cancellationToken)
  {
    TState? state = await _host.GetStateAsync(cancellationToken);

    if (state is not null)
      return state;

    if (!_descriptor.IsVirtual)
      throw new UninitializedActorException(typeof(TActor));

    TState defaultValue = _descriptor.State.GetDefaultValue();
    if (_descriptor.IsPolymorphic)
    {
      defaultValue.ImplementationId = _descriptor.DefaultImplementation.Id;
    }
    _descriptor.Id.SetId(defaultValue, _host.ActorId);

    return defaultValue;
  }

  private async Task ExecuteBindingsAsync(CancellationToken cancellationToken)
  {
    EnsureActorInitialized();
    EnsureStateInitialized();

    ExecutionSection previousSection = _section;
    _section = ExecutionSection.Bindings;

    for (int index = 0; index < _bindings.Count; index++)
    {
      IActorBinding<TActor> binding = _bindings[index];

      if (!_state.IsBindingEnabled(index))
        continue;

      await binding.ExecuteAsync(_actor, cancellationToken);
    }

    _section = previousSection;
  }

  private void VerifyBindingPreconditions(TActor actor)
  {
    foreach (IActorBinding<TActor> binding in _bindings)
    {
      binding.VerifyPreCondition(actor);
    }
  }

  private void EnableBindingCore(TState state, int index)
  {
    state.EnabledBindings |= (1 << index);
  }

  private void DisableBindingCore(TState state, int index)
  {
    state.EnabledBindings &= ~(1 << index);
  }

  [Conditional("DEBUG")]
  [MemberNotNull(nameof(_actor))]
  private void EnsureActorInitialized()
  {
    Debug.Assert(_actor is not null, "Actor was not initialized.");
  }

  [Conditional("DEBUG")]
  [MemberNotNull(nameof(_state))]
  private void EnsureStateInitialized()
  {
    Debug.Assert(_state is not null, "Actor state was not initialized.");
  }

  private sealed class DynamicArgumentList : IReadOnlyList<object?>
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
