using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IActorHost<TActor>
  where TActor : class
{
  TActor Actor { get; }

  string ActorId { get; }

  Task ScheduleAsync(string activityName, IReadOnlyList<object?> arguments, SchedulingOptions options, CancellationToken cancellationToken = default);

  void Become(string discriminator);
}
