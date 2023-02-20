using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

[ActorImplementation<TestActor>]
public class TestActor2 : TestActor
{
  public TestActor2(TestActorState state, IActorContext<TestActor> context, ActorOutput output)
    : base(state, context, output)
  {
  }

  public override ValueTask<int> PolymorphicMethodAsync(CancellationToken cancellationToken = default)
  {
    return ValueTask.FromResult(2);
  }
}
