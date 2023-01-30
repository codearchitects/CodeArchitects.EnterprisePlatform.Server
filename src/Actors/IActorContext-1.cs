using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors;

public interface IActorContext<TActor> : IActorContext
  where TActor : class
{
  Task ScheduleAsync(Expression<Func<TActor, Task>> activity);

  IActorContext<TSuper> For<TSuper>()
    where TSuper : class;
}
