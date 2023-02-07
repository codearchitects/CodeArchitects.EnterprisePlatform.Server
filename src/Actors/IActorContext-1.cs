using CodeArchitects.Platform.Actors.Scheduling;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors;

public interface IActorContext<TActor> : IActorContext
  where TActor : class
{
  void Become<TImplementation>()
    where TImplementation : class, TActor;

  Task<ScheduleId> ScheduleAsync(Expression<Func<TActor, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default);

  Task<ScheduleId> ScheduleAsync<TImplementation>(Expression<Func<TImplementation, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor;
}
