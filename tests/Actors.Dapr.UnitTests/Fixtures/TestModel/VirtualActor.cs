using CodeArchitects.Platform.Actors.Dapr.Factory;
using CodeArchitects.Platform.Actors.Dapr.Infrastructure;
using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.TestModel;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

namespace CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;

internal interface IVirtualActorHost : IActor
{
}

internal class VirtualActorHost : DaprActorHost<VirtualActor, VirtualActorState>, IVirtualActorHost
{
  public VirtualActorHost(ActorHost host, IManagerFactory<VirtualActor, VirtualActorState> factory)
    : base(host, factory)
  {
  }
}

internal class VirtualActorProxy : IVirtualActor
{
  private readonly IVirtualActorHost _actorHost;

  public VirtualActorProxy(IVirtualActorHost actorHost)
  {
    _actorHost = actorHost;
  }
}

internal class VirtualActorProxyFactory : ProxyFactory<IVirtualActorHost, string, IVirtualActor, VirtualActorState>, IVirtualActorFactory
{
  public VirtualActorProxyFactory(IActorProxyFactory actorFactory)
    : base(actorFactory)
  {
  }

  protected override string ActorName => nameof(VirtualActor);

  public IVirtualActor Get(string id)
  {
    return GetCore(id);
  }

  protected override IVirtualActor CreateProxy(IVirtualActorHost actorHost)
  {
    return new VirtualActorProxy(actorHost);
  }

  protected override Task InitAsync(IVirtualActorHost actorHost, VirtualActorState state, CancellationToken cancellationToken)
  {
    throw new NotSupportedException();
  }
}

internal class VirtualActorDaprFixture
{
  public static readonly IActorHostEmitResult HostEmitResult;

  static VirtualActorDaprFixture()
  {
    IActorDescriptor descriptor = VirtualActorFixture.Descriptor;

    Mock<IActorHostEmitResult> hostEmitResultMock = new(MockBehavior.Strict);

    hostEmitResultMock
      .Setup(x => x.InterfaceType)
      .Returns(new ActorHostTypeDelegator(typeof(IVirtualActorHost)));
    hostEmitResultMock
      .Setup(x => x.ClassType)
      .Returns(typeof(VirtualActorHost));

    HostEmitResult = hostEmitResultMock.Object;
  }
}