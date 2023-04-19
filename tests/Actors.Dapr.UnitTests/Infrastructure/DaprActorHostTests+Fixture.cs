using CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.Emit.Reflection;
using Dapr.Actors.Runtime;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

public class DaprActorHostFixture
{
  public const string ActorId = "myId";

  internal DaprActorHostFixture(
    Mock<ActorTimerManager> timerManagerMock,
    Mock<IActorStateManager> stateManagerMock,
    Mock<IActorManager<StandardActor, StandardActorState>> managerMock,
    Mock<IManagerFactory<StandardActor, StandardActorState>> factoryMock)
  {
    TimerManagerMock = timerManagerMock;
    StateManagerMock = stateManagerMock;
    ManagerMock = managerMock;
    FactoryMock = factoryMock;
  }

  internal Mock<ActorTimerManager> TimerManagerMock { get; }
  internal Mock<IActorStateManager> StateManagerMock { get; }
  internal Mock<IActorManager<StandardActor, StandardActorState>> ManagerMock { get; }
  internal Mock<IManagerFactory<StandardActor, StandardActorState>> FactoryMock { get; }

  public class HostDataAttribute : DataAttribute
  {    
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      PropertyInfo stateManagerProperty = typeof(Actor).GetRequiredProperty(
        name: "StateManager",
        bindingAttr: BindingFlags.Instance | BindingFlags.Public);

      Mock<ActorTimerManager> timerManagerMock = new(MockBehavior.Strict);
      Mock<IActorStateManager> stateManagerMock = new(MockBehavior.Strict);
      Mock<IActorManager<StandardActor, StandardActorState>> managerMock = new(MockBehavior.Strict);
      Mock<IManagerFactory<StandardActor, StandardActorState>> factoryMock = new(MockBehavior.Strict);

      ActorHost host = ActorHost.CreateForTest<StandardActorHost>(new ActorTestOptions
      {
        ActorId = new(ActorId),
        TimerManager = timerManagerMock.Object
      });

      StandardActorHost sut1 = new(host, factoryMock.Object);
      stateManagerProperty.SetValue(sut1, stateManagerMock.Object);
      sut1.Manager = managerMock.Object;
      yield return new object?[] { new DaprActorHostFixture(timerManagerMock, stateManagerMock, managerMock, factoryMock), sut1 };

      ActorHostTypeBuilder actorTypeBuilder = new(DynamicAssembly.NewModule(), new ReflectionILGeneratorProvider());
      ActorHostEmitResult emitResult = actorTypeBuilder.Build(StandardActorFixture.Descriptor, nameof(StandardActor));

      var sut2 = (DaprActorHost<StandardActor, StandardActorState>)Activator.CreateInstance(emitResult.ClassType, new object?[] { host, factoryMock.Object })!;
      sut2.Manager = managerMock.Object;
      stateManagerProperty.SetValue(sut2, stateManagerMock.Object);
      yield return new object?[] { new DaprActorHostFixture(timerManagerMock, stateManagerMock, managerMock, factoryMock), sut2 };
    }
  }
}
