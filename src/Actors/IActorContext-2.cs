using CodeArchitects.Platform.Actors.Scheduling;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors;

internal interface IActorContext<TActor, TImplementation> : IActorContext<TActor>
  where TActor : class
  where TImplementation : class, TActor
{
  Task<ScheduleId> ScheduleAsync(Expression<Func<TImplementation, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default);
}
