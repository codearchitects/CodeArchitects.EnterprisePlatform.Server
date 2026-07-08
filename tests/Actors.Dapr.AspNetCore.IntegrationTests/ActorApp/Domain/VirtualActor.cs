using CodeArchitects.Platform.Actors;
using CodeArchitects.Platform.Actors.Bindings;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Messaging;

namespace ActorApp.Domain;

[Actor, Virtual]
public abstract class VirtualActor : IVirtualActor, IMessageHandler<TestMessage>
{
  protected readonly BindingId _binding;

  [ActorId] protected readonly Guid _id;
  [State] protected readonly VirtualActorState _state;
  protected readonly IActorContext<VirtualActor> _context;
  protected readonly ActorOutput _output;

  protected VirtualActor(Guid id, VirtualActorState state, IActorContext<VirtualActor> context, ActorOutput output)
  {
    _id = id;
    _state = state;
    _context = context;
    _output = output;

    _binding = context.RegisterBinding(_ => _
      .WithPostCondition(self => self._state.ExecuteBinding)
      .IsEnabled()
      .BindTo(self => self.ExecuteBinding("binding")));
  }

  public abstract ValueTask<int> PolymorphicMethodAsync(CancellationToken cancellationToken = default);

  public Task BecomeAsync(int implementation, CancellationToken cancellationToken = default)
  {
    switch (implementation)
    {
      case 1:
        _context.Become<VirtualActor1>();
        return Task.CompletedTask;
      case 2:
        _context.Become<VirtualActor2>();
        return Task.CompletedTask;
      default:
        throw new ArgumentOutOfRangeException(nameof(implementation));
    }
  }

  public Task BindingEnablerAsync(CancellationToken cancellationToken = default)
  {
    _state.ExecuteBinding = true;
    return Task.CompletedTask;
  }

  public Task BindingDisablerAsync(CancellationToken cancellationToken = default)
  {
    _context.DisableBinding(_binding);
    _state.ExecuteBinding = true;
    return Task.CompletedTask;
  }

  public async Task ScheduleAsync(string output, CancellationToken cancellationToken = default)
  {
    await _context.ScheduleAsync(self => self.Activity(output), SchedulingOptions.In(5.Seconds()), cancellationToken);
  }

  public Task HandleAsync(TestMessage message, CancellationToken cancellationToken)
  {
    _output.SetOutput(_id, message.Output);
    return Task.CompletedTask;
  }

  private void ExecuteBinding(string output)
  {
    _state.ExecuteBinding = false;
    _output.SetOutput(_id, output);
  }

  private void Activity(string output)
  {
    _output.SetOutput(_id, output);
  }
}
