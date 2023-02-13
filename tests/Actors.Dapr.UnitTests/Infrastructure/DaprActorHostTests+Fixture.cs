using CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.Emit.Reflection;
using Dapr.Actors.Runtime;
using Moq;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

public partial class DaprActorHostTests
{
  public class HostDataAttribute : DataAttribute
  {
    public const string Id = "myId";
    
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      PropertyInfo stateManagerProperty = typeof(Actor).GetRequiredProperty(
        name: "StateManager",
        bindingAttr: BindingFlags.Instance | BindingFlags.Public);

      Mock<ActorTimerManager> timerManagerMock = new(MockBehavior.Strict);
      Mock<IActorStateManager> stateManagerMock = new(MockBehavior.Strict);
      Mock<IActorManager<StandardActor, StandardActorState>> managerMock = new(MockBehavior.Strict);
      Mock<IImplementationFactory<StandardActor, StandardActorState>> factoryMock = new(MockBehavior.Strict);

      ActorHost host = ActorHost.CreateForTest<StandardActorHost>(new ActorTestOptions
      {
        ActorId = new(Id),
        TimerManager = timerManagerMock.Object
      });

      StandardActorHost sut1 = new(host, managerMock.Object, factoryMock.Object);
      stateManagerProperty.SetValue(sut1, stateManagerMock.Object);
      yield return new object?[] { timerManagerMock, stateManagerMock, managerMock, factoryMock, sut1 };

      ActorHostTypeBuilder actorTypeBuilder = new(DynamicAssembly.NewModule(), new ReflectionILGeneratorProvider());
      ActorHostEmitResult emitResult = actorTypeBuilder.Build(StandardActorFixture.Descriptor, nameof(StandardActor));

      object sut2 = Activator.CreateInstance(emitResult.ClassType, new object?[] { host, managerMock.Object, factoryMock.Object })!;
      stateManagerProperty.SetValue(sut2, stateManagerMock.Object);
      yield return new object?[] { timerManagerMock, stateManagerMock, managerMock, factoryMock, sut2 };
    }
  }
}
