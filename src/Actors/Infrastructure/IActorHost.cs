using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IActorHost<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  string ActorId { get; }

  Task ScheduleAsync(ScheduleId id, Activity<TActor> activity, SchedulingOptions options, CancellationToken cancellationToken);

  Task UnscheduleAsync(ScheduleId id, CancellationToken cancellationToken);

  Task<TState?> GetStateAsync(CancellationToken cancellationToken);

  Task SetStateAsync(TState state, CancellationToken cancellationToken);
}
