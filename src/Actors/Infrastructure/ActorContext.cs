using CodeArchitects.Platform.Actors.Bindings;
using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Scheduling;
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
  private readonly IActivityManager _activityManager;
  private readonly IActorHost<TActor, TState> _host;
  private readonly int _implementationId;
  private readonly List<ActorBinding<TActor>> _bindings;
  private ExecutionSection _section;

  public ActorContext(
    IServiceProvider services,
    IActorDescriptor<TActor, TState> descriptor,
    IActivityManager activityManager,
    IActorHost<TActor, TState> host,
    TState state,
    int implementationId)
  {
    Actor = descriptor.CreateInstance(implementationId, services, state, this);
    _descriptor = descriptor;
    _activityManager = activityManager;
    _host = host;
    State = state;
    _implementationId = implementationId;
    _bindings = new();
    _section = ExecutionSection.Constructor;
  }

  public TActor Actor { get; }

  public TState State { get; }

  public string ActorId => _host.ActorId;

  public int DefaultImplementationId => _descriptor.DefaultImplementation.Id;

  public JsonSerializerOptions JsonSerializerOptions => _descriptor.JsonSerializerOptions;

  public Type ActivityType => _descriptor.ActivityBaseType;

  public TState DefaultState
  {
    get
    {
      if (!_descriptor.IsVirtual)
        throw new UninitializedActorException(typeof(TActor));

      return _descriptor.State.DefaultValue!;
    }
  }

  public void Become<TImplementation>()
    where TImplementation : class, TActor
  {
    State.ImplementationId = _descriptor.GetImplementation(typeof(TImplementation)).Id;
  }

  public BindingId RegisterBinding(Func<IBindingBuilder<TActor>, IBindingResult> configure)
  {
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));
    if (_section is not ExecutionSection.Constructor)
      throw new InvalidOperationException("Bindings may be only registered inside the actor's constructor.");
    
    int index = _bindings.Count;
    if (index >= 32)
      throw new InvalidOperationException("Exceeded the maximum number of bindings.");

    ActorBinding<TActor> binding = new();
    configure(binding);

    BindingId id = new(_bindings.Count);
    _bindings.Add(binding);

    return id;
  }

  public void EnableBinding(BindingId id)
  {
    State.EnabledBindings |= (1 << id._index);
  }

  public void DisableBinding(BindingId id)
  {
    State.EnabledBindings &= ~(1 << id._index);
  }

  public Task<ScheduleId> ScheduleAsync(Expression<Func<TActor, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleCoreAsync(activity, options, cancellationToken);
  }

  public Task<ScheduleId> ScheduleAsync(Expression<Action<TActor>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleCoreAsync(activity, options, cancellationToken);
  }

  public Task<ScheduleId> ScheduleAsync<TImplementation>(Expression<Func<TImplementation, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleCoreAsync(activity, options, cancellationToken);
  }

  public Task<ScheduleId> ScheduleAsync<TImplementation>(Expression<Action<TImplementation>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleCoreAsync(activity, options, cancellationToken);
  }

  public Task<ScheduleId> ScheduleAsync(string activityName, IReadOnlyList<object?>? arguments = null, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activityName is null)
      throw new ArgumentNullException(nameof(activityName));

    Activity<TActor> activity = _activityManager.CreateActivity<TActor>(_implementationId, activityName, arguments ?? Array.Empty<object?>());
    return ScheduleCoreAsync(activity, options, cancellationToken);
  }

  public async Task UnscheduleAsync(ScheduleId id, CancellationToken cancellationToken = default)
  {
    await _host.UnscheduleAsync(id, cancellationToken);
  }

  private Task<ScheduleId> ScheduleCoreAsync(LambdaExpression activityExpression, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    ParameterExpression actorParam = activityExpression.Parameters[0];

    if (activityExpression.Body is not MethodCallExpression methodCallExpression || methodCallExpression.Object != actorParam)
      throw new ArgumentException($"The '{nameof(activityExpression)}' expression should represent a call to an instance method of the current actor.", nameof(activityExpression));

    DynamicArgumentList arguments = new(methodCallExpression.Arguments); // Avoids allocating an array
    Activity<TActor> activity = _activityManager.CreateActivity<TActor>(_implementationId, methodCallExpression.Method, arguments);

    return ScheduleCoreAsync(activity, options, cancellationToken);
  }

  private async Task<ScheduleId> ScheduleCoreAsync(Activity<TActor> activity, SchedulingOptions? options, CancellationToken cancellationToken = default)
  {
    if (options is null)
    {
      options = new(ScheduleId.New(), TimeSpan.Zero, Timeout.InfiniteTimeSpan);
    }
    else if (options.ScheduleId == default)
    {
      options = options with { ScheduleId = ScheduleId.New() };
    }

    await _host.ScheduleAsync(activity, options, cancellationToken);

    return options.ScheduleId;
  }

  public void OnActivityBegin()
  {
    _section = ExecutionSection.Activity;
  }

  public void OnMethodBegin()
  {
    _section = ExecutionSection.Method;
  }

  public async Task OnExecutionEndAsync(CancellationToken cancellationToken)
  {
    _section = ExecutionSection.Binding;

    for (int i = 0; i < _bindings.Count; i++)
    {
      ActorBinding<TActor> binding = _bindings[i];
      bool isEnabled = (State.EnabledBindings & (1 << i)) != 0;

      if (!isEnabled && !binding.IsEnabled)
        continue;

      await binding.ExecuteAsync(Actor, cancellationToken);
    }

    _section = ExecutionSection.None;
    _descriptor.UpdateState(Actor, State);
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
