using CodeArchitects.Platform.Actors.Dapr.Factory;
using CodeArchitects.Platform.Actors.Dapr.Infrastructure;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.TestModel;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

namespace CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;

internal interface IComponentIdSourceActorHost : IActor
{
  Task _InitAsync(ComponentIdSourceActorState state, CancellationToken cancellationToken);
}

internal class ComponentIdSourceActorHost : DaprActorHost<ComponentIdSourceActor, ComponentIdSourceActorState>, IComponentIdSourceActorHost
{
  public ComponentIdSourceActorHost(ActorHost host, IManagerFactory<ComponentIdSourceActor, ComponentIdSourceActorState> factory)
    : base(host, factory)
  {
  }

  public Task _InitAsync(ComponentIdSourceActorState state, CancellationToken cancellationToken)
  {
    return InitAsync(state, cancellationToken);
  }
}

internal class ComponentIdSourceActorProxy : IComponentIdSourceActor
{
  private readonly IComponentIdSourceActorHost _actorHost;

  public ComponentIdSourceActorProxy(IComponentIdSourceActorHost actorHost)
  {
    _actorHost = actorHost;
  }
}

internal class ComponentIdSourceActorProxyFactory : ProxyFactory<IComponentIdSourceActorHost, int, IComponentIdSourceActor, ComponentIdSourceActorState>, IComponentIdSourceActorFactory
{
  public ComponentIdSourceActorProxyFactory(IActorProxyFactory actorFactory)
    : base(actorFactory)
  {
  }

  protected override string ActorName => nameof(ComponentIdSourceActor);

  public Task<IComponentIdSourceActor> CreateAsync(int state, CancellationToken cancellationToken = default)
  {
    return CreateCoreAsync(state, new ComponentIdSourceActorState { _0 = state }, cancellationToken);
  }

  public IComponentIdSourceActor Get(int id)
  {
    return GetCore(id);
  }

  protected override IComponentIdSourceActor CreateProxy(IComponentIdSourceActorHost actorHost)
  {
    return new ComponentIdSourceActorProxy(actorHost);
  }

  protected override Task InitAsync(IComponentIdSourceActorHost actorHost, ComponentIdSourceActorState state, CancellationToken cancellationToken)
  {
    return actorHost._InitAsync(state, cancellationToken);
  }
}

internal class ComponentIdSourceActorDaprFixture
{
  public static readonly IActorHostEmitResult HostEmitResult;

  static ComponentIdSourceActorDaprFixture()
  {
    IActorDescriptor descriptor = ComponentIdSourceActorFixture.Descriptor;

    Mock<IActorHostEmitResult> hostEmitResultMock = new(MockBehavior.Strict);

    hostEmitResultMock
      .Setup(x => x.InterfaceType)
      .Returns(new ActorHostTypeDelegator(typeof(IComponentIdSourceActorHost)));
    hostEmitResultMock
      .Setup(x => x.ClassType)
      .Returns(typeof(ComponentIdSourceActorHost));

    HostEmitResult = hostEmitResultMock.Object;
  }
}