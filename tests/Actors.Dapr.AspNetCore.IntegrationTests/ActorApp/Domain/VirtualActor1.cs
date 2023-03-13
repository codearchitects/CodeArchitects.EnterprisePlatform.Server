using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

[ActorImplementation<VirtualActor>]
public class VirtualActor1 : VirtualActor
{
  public VirtualActor1(VirtualActorState state, IActorContext<VirtualActor> context, ActorOutput output)
    : base(state, context, output)
  {
  }

  public override ValueTask<int> PolymorphicMethodAsync(CancellationToken cancellationToken = default)
  {
    return ValueTask.FromResult(1);
  }
}
