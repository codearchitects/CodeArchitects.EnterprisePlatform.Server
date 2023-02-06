using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Common.Collections;
using CodeArchitects.Platform.Common.Expressions;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal class ActorContext<TActor, TImplementation> : IActorContext<TActor, TImplementation>
  where TActor : class
  where TImplementation : class, TActor
{
  private readonly IActivityManager _activityManager;
  private readonly IActorHost<TActor> _host;

  public ActorContext(IActivityManager activityManager, IActorHost<TActor> host)
  {
    _activityManager = activityManager;
    _host = host;
  }

  public string ActorId => _host.ActorId;

  public void Become<TOther>()
    where TOther : TActor
  {
    throw new NotImplementedException();
  }

  public Task<ScheduleId> ScheduleAsync(Expression<Func<TImplementation, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleAsync<TImplementation>(activity, options, cancellationToken);
  }

  public Task<ScheduleId> ScheduleAsync(Expression<Func<TActor, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleAsync<TActor>(activity, options, cancellationToken);
  }

  public Task<ScheduleId> ScheduleAsync(string activityName, IReadOnlyList<object?>? arguments = null, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activityName is null)
      throw new ArgumentNullException(nameof(activityName));

    ActivityPayload payload = _activityManager.CreatePayload(activityName, arguments ?? Array.Empty<object?>());
    return ScheduleCoreAsync(payload, options, cancellationToken);
  }

  public async Task UnscheduleAsync(ScheduleId id, CancellationToken cancellationToken = default)
  {
    await _host.UnscheduleAsync(id, cancellationToken);
  }

  private Task<ScheduleId> ScheduleAsync<T>(Expression<Func<T, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where T : class
  {
    ParameterExpression actorParam = activity.Parameters[0];

    if (activity.Body is not MethodCallExpression methodCall || methodCall.Object != actorParam)
      throw new ArgumentException($"The '{nameof(activity)}' expression should represent a call to an instance method of the current actor.", nameof(activity));

    object?[] arguments = methodCall.Arguments.Map(argument => ExpressionEvaluator.Evaluate(argument));
    ActivityPayload payload = _activityManager.CreatePayload(methodCall.Method, arguments);

    return ScheduleCoreAsync(payload, options, cancellationToken);
  }

  private async Task<ScheduleId> ScheduleCoreAsync(ActivityPayload payload, SchedulingOptions? options, CancellationToken cancellationToken = default)
  {
    if (options is null)
    {
      options = new(ScheduleId.New(), TimeSpan.Zero, Timeout.InfiniteTimeSpan, true);
    }
    else if (options.ScheduleId == default)
    {
      options = options with { ScheduleId = ScheduleId.New() };
    }

    await _host.ScheduleAsync(payload, options, cancellationToken);

    return options.ScheduleId;
  }
}
