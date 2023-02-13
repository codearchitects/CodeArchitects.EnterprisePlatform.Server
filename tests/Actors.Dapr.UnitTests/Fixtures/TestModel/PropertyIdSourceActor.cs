using CodeArchitects.Platform.Actors.Dapr.Factory;
using CodeArchitects.Platform.Actors.Dapr.Infrastructure;
using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.TestModel;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

namespace CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;

internal interface IPropertyIdSourceActorHost : IActor
{
  Task _InitAsync(PropertyIdSourceActorState state, CancellationToken cancellationToken);
}

internal class PropertyIdSourceActorHost : DaprActorHost<PropertyIdSourceActor, PropertyIdSourceActorState>, IPropertyIdSourceActorHost
{
  public PropertyIdSourceActorHost(ActorHost host, IActorManager<PropertyIdSourceActor, PropertyIdSourceActorState> manager, IImplementationFactory<PropertyIdSourceActor, PropertyIdSourceActorState> factory)
    : base(host, manager, factory)
  {
  }

  public Task _InitAsync(PropertyIdSourceActorState state, CancellationToken cancellationToken)
  {
    return InitAsync(state, cancellationToken);
  }
}

internal class PropertyIdSourceActorProxy : IPropertyIdSourceActor
{
  private readonly IPropertyIdSourceActorHost _actorHost;

  public PropertyIdSourceActorProxy(IPropertyIdSourceActorHost actorHost)
  {
    _actorHost = actorHost;
  }
}

internal class PropertyIdSourceActorProxyFactory : ProxyFactory<IPropertyIdSourceActorHost, IPropertyIdSourceActor, PropertyIdSourceActorState>, IPropertyIdSourceActorFactory
{
  public PropertyIdSourceActorProxyFactory(IActorProxyFactory actorFactory)
    : base(actorFactory)
  {
  }

  protected override string ActorName => nameof(PropertyIdSourceActor);

  public Task<IPropertyIdSourceActor> CreateAsync(PropertyIdSourceActorStateComponent state, CancellationToken cancellationToken = default)
  {
    return CreateCoreAsync(state.Id, new PropertyIdSourceActorState { _state = state }, cancellationToken);
  }

  public IPropertyIdSourceActor Get(int id)
  {
    return GetCore(id);
  }

  protected override IPropertyIdSourceActor CreateProxy(IPropertyIdSourceActorHost actorHost)
  {
    return new PropertyIdSourceActorProxy(actorHost);
  }

  protected override Task InitAsync(IPropertyIdSourceActorHost actorHost, PropertyIdSourceActorState state, CancellationToken cancellationToken)
  {
    return actorHost._InitAsync(state, cancellationToken);
  }
}

internal class PropertyIdSourceActorDaprFixture
{
  public static readonly IActorHostEmitResult HostEmitResult;

  static PropertyIdSourceActorDaprFixture()
  {
    IActorDescriptor descriptor = PropertyIdSourceActorFixture.Descriptor;

    Mock<IActorHostEmitResult> hostEmitResultMock = new(MockBehavior.Strict);

    hostEmitResultMock
      .Setup(x => x.InterfaceType)
      .Returns(new ActorHostTypeDelegator(typeof(IPropertyIdSourceActorHost)));
    hostEmitResultMock
      .Setup(x => x.ClassType)
      .Returns(typeof(PropertyIdSourceActorHost));

    HostEmitResult = hostEmitResultMock.Object;
  }
}