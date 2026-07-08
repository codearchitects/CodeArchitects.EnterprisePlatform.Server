using CodeArchitects.Platform.Actors.Bindings;
using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors.Infrastructure;

public partial class ActorContextTests
{
  internal class TestActor1
  {
    public void Activity(string arg)
    {
    }

    public void Activity(int arg)
    {
    }
  }

  internal class TestActor1State : PolymorphicActorState
  {
  }

  internal class TestActor1Impl : TestActor1
  {
  }

  internal class TestActor1Activity : Activity<TestActor1>
  {
    public override int Id => throw new NotImplementedException();

    public override Task ExecuteAsync(TestActor1 actor, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }

  internal class TestActor2
  {
    public TestActor2(TestActor2State state, IActorContext<TestActor2> context)
    {
      State = state;

      BindingId = context.RegisterBinding(binding => binding
        .WithPreCondition(self => self.State.Value == 1)
        .WithPostCondition(self => self.State.Value == 2)
        .IsEnabled()
        .BindTo(self => self.BindingExecuted = true));
    }

    public TestActor2State State { get; }

    public BindingId BindingId { get; private set; }

    public bool BindingExecuted { get; private set; }
  }

  internal class TestActor2State : PolymorphicActorState
  {
    public int Value { get; set; }
  }
}
