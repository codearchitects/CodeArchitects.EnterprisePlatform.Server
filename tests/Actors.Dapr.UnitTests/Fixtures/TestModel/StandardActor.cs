using CodeArchitects.Platform.Actors.Dapr.Factory;
using CodeArchitects.Platform.Actors.Dapr.Infrastructure;
using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.TestModel;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;

internal interface IStandardActorHost : IActor
{
  Task TaskMethod_1(int arg);

  Task<int> TaskMethod_2(int arg, CancellationToken cancellationToken);

  Task ValueTaskMethod(CancellationToken cancellationToken);

  Task<string> ValueTaskTMethod();

  Task _InitAsync(StandardActorState state, CancellationToken cancellationToken);
}

internal class StandardActorHost : DaprActorHost<StandardActor, StandardActorState>, IStandardActorHost
{
  public StandardActorHost(ActorHost host, IActorManager<StandardActor, StandardActorState> manager, IImplementationFactory<StandardActor, StandardActorState> factory)
    : base(host, manager, factory)
  {
  }

  public Task TaskMethod_1(int arg)
  {
    return Actor.TaskMethod(arg);
  }

  public Task<int> TaskMethod_2(int arg, CancellationToken cancellationToken)
  {
    return Actor.TaskMethod(arg, cancellationToken);
  }

  public Task ValueTaskMethod(CancellationToken cancellationToken)
  {
    return Actor.ValueTaskMethod(cancellationToken).AsTask();
  }

  public Task<string> ValueTaskTMethod()
  {
    return Actor.ValueTaskTMethod().AsTask();
  }

  public Task _InitAsync(StandardActorState state, CancellationToken cancellationToken)
  {
    return InitAsync(state, cancellationToken);
  }
}

internal class StandardActorProxy : IStandardActor
{
  private readonly IStandardActorHost _actorHost;

  public StandardActorProxy(IStandardActorHost actorHost)
  {
    _actorHost = actorHost;
  }

  public Task TaskMethod(int arg)
  {
    return _actorHost.TaskMethod_1(arg);
  }

  public Task<int> TaskMethod(int arg, CancellationToken cancellationToken)
  {
    return _actorHost.TaskMethod_2(arg, cancellationToken);
  }

  public ValueTask ValueTaskMethod(CancellationToken cancellationToken)
  {
    return new ValueTask(_actorHost.ValueTaskMethod(cancellationToken));
  }

  public ValueTask<string> ValueTaskTMethod()
  {
    return new ValueTask<string>(_actorHost.ValueTaskTMethod());
  }
}

internal class StandardActorProxyFactory : ProxyFactory<IStandardActorHost, IStandardActor, StandardActorState>, IStandardActorFactory
{
  public StandardActorProxyFactory(IActorProxyFactory actorFactory)
    : base(actorFactory)
  {
  }

  protected override string ActorName => nameof(StandardActor);

  public Task<IStandardActor> CreateAsync(string id, string state1, StandardActorStateComponent state2, CancellationToken cancellationToken = default)
  {
    return CreateCoreAsync(id, new StandardActorState { _state1 = state1, _state2 = state2 }, cancellationToken);
  }

  public IStandardActor Get(string id)
  {
    return GetCore(id);
  }

  protected override IStandardActor CreateProxy(IStandardActorHost actorHost)
  {
    return new StandardActorProxy(actorHost);
  }

  protected override Task InitAsync(IStandardActorHost actorHost, StandardActorState state, CancellationToken cancellationToken)
  {
    return actorHost._InitAsync(state, cancellationToken);
  }
}

internal class StandardActorDaprFixture
{
  public static readonly MethodInfo HostTaskMethod;
  public static readonly MethodInfo HostTaskTMethod;
  public static readonly MethodInfo HostValueTaskMethod;
  public static readonly MethodInfo HostValueTaskTMethod;

  public static readonly IActorHostEmitResult HostEmitResult;

  static StandardActorDaprFixture()
  {
    HostTaskMethod = typeof(IStandardActorHost).GetRequiredMethod(nameof(IStandardActorHost.TaskMethod_1));
    HostTaskTMethod = typeof(IStandardActorHost).GetRequiredMethod(nameof(IStandardActorHost.TaskMethod_2));
    HostValueTaskMethod = typeof(IStandardActorHost).GetRequiredMethod(nameof(IStandardActorHost.ValueTaskMethod));
    HostValueTaskTMethod = typeof(IStandardActorHost).GetRequiredMethod(nameof(IStandardActorHost.ValueTaskTMethod));

    IActorDescriptor descriptor = StandardActorFixture.Descriptor;

    Mock<IActorHostEmitResult> hostEmitResultMock = new(MockBehavior.Strict);

    hostEmitResultMock
      .Setup(x => x.InterfaceType)
      .Returns(new ActorHostTypeDelegator(typeof(IStandardActorHost)));
    hostEmitResultMock
      .Setup(x => x.ClassType)
      .Returns(typeof(StandardActorHost));
    hostEmitResultMock
      .Setup(x => x.GetHostMethod(descriptor.BaseImplementation.Methods[0]))
      .Returns(HostTaskMethod);
    hostEmitResultMock
      .Setup(x => x.GetHostMethod(descriptor.BaseImplementation.Methods[1]))
      .Returns(HostTaskTMethod);
    hostEmitResultMock
      .Setup(x => x.GetHostMethod(descriptor.BaseImplementation.Methods[2]))
      .Returns(HostValueTaskMethod);
    hostEmitResultMock
      .Setup(x => x.GetHostMethod(descriptor.BaseImplementation.Methods[3]))
      .Returns(HostValueTaskTMethod);

    HostEmitResult = hostEmitResultMock.Object;
  }
}