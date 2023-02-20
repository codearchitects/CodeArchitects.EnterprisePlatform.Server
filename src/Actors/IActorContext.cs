using CodeArchitects.Platform.Actors.Bindings;
using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors;

public interface IActorContext
{
  string ActorId { get; }

  Task ScheduleAsync(ActivitySpec activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default);

  Task ScheduleAsync(ScheduleId id, ActivitySpec activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default);

  Task UnscheduleAsync(ScheduleId scheduleId, CancellationToken cancellationToken = default);

  void EnableBinding(BindingId id);

  void DisableBinding(BindingId id);
}
