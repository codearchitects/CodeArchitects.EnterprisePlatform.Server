using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors;

public interface IActorContext
{
  string ActorId { get; }

  Task<ScheduleId> ScheduleAsync(string activityName, IReadOnlyList<object?>? arguments = null, SchedulingOptions? options = null, CancellationToken cancellationToken = default);
}
