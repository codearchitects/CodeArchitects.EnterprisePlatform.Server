using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

[ActorImplementation<TestActor>]
public class TestActor1 : TestActor
{
  public TestActor1(TestActorState state, IActorContext<TestActor> context, ActorOutput output)
    : base(state, context, output)
  {
  }

  public override ValueTask<int> PolymorphicMethodAsync(CancellationToken cancellationToken = default)
  {
    return ValueTask.FromResult(1);
  }
}
