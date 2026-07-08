namespace ActorApp.Domain;

public interface IVirtualActor
{
  Task BecomeAsync(int implementation, CancellationToken cancellationToken = default);

  Task ScheduleAsync(string output, CancellationToken cancellationToken = default);

  ValueTask<int> PolymorphicMethodAsync(CancellationToken cancellationToken = default);

  Task BindingEnablerAsync(CancellationToken cancellationToken = default);

  Task BindingDisablerAsync(CancellationToken cancellationToken = default);
}