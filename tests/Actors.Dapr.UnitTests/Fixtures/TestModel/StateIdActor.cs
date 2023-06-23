using CodeArchitects.Platform.Actors.Dapr.Factory;
using CodeArchitects.Platform.Actors.Dapr.Infrastructure;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.TestModel;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

namespace CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;

internal interface IStateIdActorHost : IActor
{
  Task _InitAsync(byte[] payload, CancellationToken cancellationToken);
}

internal class StateIdActorHost : DaprActorHost<StateIdActor, StateIdActorState>, IStateIdActorHost
{
  public StateIdActorHost(ActorHost host, IActorManagerFactory<StateIdActor, StateIdActorState> factory)
    : base(host, factory)
  {
  }

  public Task _InitAsync(byte[] payload, CancellationToken cancellationToken)
  {
    return InitAsync(payload, cancellationToken);
  }
}

internal class StateIdActorProxy : IStateIdActor
{
  private readonly IStateIdActorHost _actorHost;

  public StateIdActorProxy(IStateIdActorHost actorHost)
  {
    _actorHost = actorHost;
  }
}

internal class StateIdActorProxyFactory : ProxyFactory<IStateIdActorHost, int, IStateIdActor, StateIdActorState>, IStateIdActorFactory
{
  public StateIdActorProxyFactory(IActorProxyFactory actorFactory)
    : base(actorFactory)
  {
  }

  protected override string ActorName => nameof(StateIdActor);

  public Task<IStateIdActor> CreateAsync(int state, CancellationToken cancellationToken = default)
  {
    return CreateAsync(state, new StateIdActorState { _0 = state }, cancellationToken);
  }

  public IStateIdActor Get(int id)
  {
    return GetCore(id);
  }

  protected override IStateIdActor CreateProxy(IStateIdActorHost actorHost)
  {
    return new StateIdActorProxy(actorHost);
  }

  protected override Task InitAsync(IStateIdActorHost actorHost, byte[] payload, CancellationToken cancellationToken)
  {
    return actorHost._InitAsync(payload, cancellationToken);
  }
}

internal class StateIdActorDaprFixture
{
  public static readonly IActorHostEmitResult HostEmitResult;

  static StateIdActorDaprFixture()
  {
    IActorDescriptor descriptor = StateIdActorFixture.Descriptor;

    Mock<IActorHostEmitResult> hostEmitResultMock = new(MockBehavior.Strict);

    hostEmitResultMock
      .Setup(x => x.InterfaceType)
      .Returns(new ActorHostTypeDelegator(typeof(IStateIdActorHost)));
    hostEmitResultMock
      .Setup(x => x.ClassType)
      .Returns(typeof(StateIdActorHost));

    HostEmitResult = hostEmitResultMock.Object;
  }
}