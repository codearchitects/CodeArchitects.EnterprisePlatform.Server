using CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;
using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.Emit.Reflection;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Actors.Dapr.Proxy;

public partial class ActorProxyTests
{
  private class StandardActorProxyDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      Mock<IStandardActorHost> actorHostMock = new(MockBehavior.Loose);

      StandardActorProxy sut1 = new(actorHostMock.Object);
      yield return new object?[] { actorHostMock, sut1 };

      ActorProxyTypeBuilder typeBuilder = new(DynamicAssembly.NewModule(), new ReflectionILGeneratorProvider());
      Type proxyType = typeBuilder.Build(StandardActorFixture.Descriptor, StandardActorDaprFixture.HostEmitResult);

      object sut2 = Activator.CreateInstance(proxyType, new[] { actorHostMock.Object })!;
      yield return new object?[] { actorHostMock, sut2 };
    }
  }
}
