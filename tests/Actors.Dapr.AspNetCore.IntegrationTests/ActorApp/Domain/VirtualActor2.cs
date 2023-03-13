using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

[ActorImplementation<VirtualActor>(IsDefault = true)]
public class VirtualActor2 : VirtualActor
{
  public VirtualActor2(VirtualActorState state, IActorContext<VirtualActor> context, ActorOutput output)
    : base(state, context, output)
  {
  }

  public override ValueTask<int> PolymorphicMethodAsync(CancellationToken cancellationToken = default)
  {
    return ValueTask.FromResult(2);
  }
}
