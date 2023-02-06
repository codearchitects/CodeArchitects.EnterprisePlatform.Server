using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IActorHost<TActor>
  where TActor : class
{
  TActor Actor { get; }

  string ActorId { get; }

  Task ScheduleAsync(ActivityPayload payload, SchedulingOptions options, CancellationToken cancellationToken);

  Task UnscheduleAsync(ScheduleId id, CancellationToken cancellationToken);
}
