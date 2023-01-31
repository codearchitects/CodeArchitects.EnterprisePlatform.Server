using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Common.Collections;
using CodeArchitects.Platform.Common.Expressions;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal class ActorContext<TActor, TImplementation> : IActorContext<TActor, TImplementation>
  where TActor : class
  where TImplementation : class, TActor
{
  private readonly IActorHost<TActor> _host;

  public ActorContext(IActorHost<TActor> host)
  {
    _host = host;
  }

  public string ActorId => _host.ActorId;

  public void Become<TOther>()
    where TOther : TActor
  {
    _host.Become(typeof(TOther).FullName);
  }

  public Task<ScheduleId> ScheduleAsync(Expression<Func<TImplementation, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleCoreAsync(activity, options, cancellationToken);
  }

  public Task<ScheduleId> ScheduleAsync(Expression<Func<TActor, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activity is null)
      throw new ArgumentNullException(nameof(activity));

    return ScheduleCoreAsync(activity, options, cancellationToken);
  }

  public Task<ScheduleId> ScheduleAsync(string activityName, IReadOnlyList<object?>? arguments = null, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
  {
    if (activityName is null)
      throw new ArgumentNullException(nameof(activityName));

    return ScheduleCoreAsync(activityName, arguments ?? Array.Empty<object?>(), options, cancellationToken);
  }

  private Task<ScheduleId> ScheduleCoreAsync<T>(Expression<Func<T, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where T : class
  {
    ParameterExpression actorParam = activity.Parameters[0];

    if (activity.Body is not MethodCallExpression methodCall || methodCall.Object != actorParam)
      throw new ArgumentException("The 'activity' expression should represent a call to an instance method of the current actor.", nameof(activity));

    string activityName = methodCall.Method.Name;
    object?[] arguments = methodCall.Arguments.Map(
      (parameters: activity.Parameters, arguments: new[] { _host.Actor }),
      static (state, argument) => ExpressionEvaluator.Evaluate(argument, state.parameters, state.arguments));

    return ScheduleCoreAsync(activityName, arguments, options, cancellationToken);
  }

  private async Task<ScheduleId> ScheduleCoreAsync(string activityName, IReadOnlyList<object?> arguments, SchedulingOptions? options, CancellationToken cancellationToken = default)
  {
    options ??= SchedulingOptions.Now;
    await _host.ScheduleAsync(activityName, arguments, options, cancellationToken);

    if (options.Id != default)
      return options.Id;

    return new ScheduleId(Guid.NewGuid().ToString());
  }
}
