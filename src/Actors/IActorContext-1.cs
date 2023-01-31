using CodeArchitects.Platform.Actors.Scheduling;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors;

public interface IActorContext<TActor> : IActorContext
  where TActor : class
{
  void Become<TOther>()
    where TOther : TActor;

  Task<ScheduleId> ScheduleAsync(Expression<Func<TActor, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default);
}
