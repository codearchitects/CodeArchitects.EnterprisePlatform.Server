using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Common.Collections;
using CodeArchitects.Platform.Common.Expressions;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal class ActorContext<TActor, TState> : IActorContext<TActor>
  where TActor : class
  where TState : ActorState
{
  private readonly IActorManager<TActor, TState> _actorManager;
  private readonly IActivityManager _activityManager;
  private readonly IActorHost<TActor, TState> _host;
  private readonly int _implementationId;

  public ActorContext(IActorManager<TActor, TState> actorManager, IActivityManager activityManager, IActorHost<TActor, TState> host, int implementationId)
  {
    _actorManager = actorManager;
    _activityManager = activityManager;
    _host = host;
    _implementationId = implementationId;
  }

  public string ActorId => _host.ActorId;

  public void Become<TImplementation>()
    where TImplementation : class, TActor
  {
    _host.State.ImplementationId = _actorManager.GetImplementationId(typeof(TImplementation));
  }

  public Task<ScheduleId> ScheduleAsync(Expression<Func<TActor, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleAsyncCore(activity, options, cancellationToken);
  }

  public Task<ScheduleId> ScheduleAsync<TImplementation>(Expression<Func<TImplementation, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleAsyncCore(activity, options, cancellationToken);
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

  private Task<ScheduleId> ScheduleAsyncCore<T>(Expression<Func<T, Task>> activityExpression, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where T : class
  {
    ParameterExpression actorParam = activityExpression.Parameters[0];

    if (activityExpression.Body is not MethodCallExpression methodCallExpression || methodCallExpression.Object != actorParam)
      throw new ArgumentException($"The '{nameof(activityExpression)}' expression should represent a call to an instance method of the current actor.", nameof(activityExpression));

    object?[] arguments = methodCallExpression.Arguments.Map(argument => ExpressionEvaluator.Evaluate(argument));
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
}
