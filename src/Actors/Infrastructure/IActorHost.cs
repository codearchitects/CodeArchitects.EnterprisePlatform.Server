using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IActorHost<TActor>
  where TActor : class
{
  string ActorId { get; }

  Task ScheduleAsync(ScheduleId id, Activity<TActor> activity, SchedulingOptions options, CancellationToken cancellationToken);

  Task UnscheduleAsync(ScheduleId id, CancellationToken cancellationToken);
}
