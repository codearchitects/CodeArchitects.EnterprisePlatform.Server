using CodeArchitects.Platform.Actors.Dapr.Factory;
using CodeArchitects.Platform.Actors.Dapr.Infrastructure;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Messaging;
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

  Task _InitAsync(byte[] payload, CancellationToken cancellationToken);
}

internal interface IStandardActorMessageHandler : IActor
{
  Task HandleAsync(ActorMessage message, CancellationToken cancellationToken);
}

internal class StandardActorHost : DaprActorHost<StandardActor, StandardActorState>, IStandardActorHost, IStandardActorMessageHandler
{
  public StandardActorHost(ActorHost host, IManagerFactory<StandardActor, StandardActorState> factory)
    : base(host, factory)
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

  public Task _InitAsync(byte[] payload, CancellationToken cancellationToken)
  {
    return InitAsync(payload, cancellationToken);
  }

  public Task HandleAsync(ActorMessage message, CancellationToken cancellationToken)
  {
    return Actor.HandleAsync(message, cancellationToken);
  }
}

[MessageHandler]
internal class StandardActorMessageHandler : ActorHostFactory<IStandardActorMessageHandler, string>, IMessageHandler<ActorMessage>
{
  public StandardActorMessageHandler(IActorProxyFactory hostFactory)
    : base(hostFactory)
  {
  }

  protected override string ActorName => nameof(StandardActor);

  [MessageHandler("bus", "topic")]
  public Task HandleAsync(ActorMessage message, CancellationToken cancellationToken)
  {
    IStandardActorMessageHandler host = CreateHost(message.ActorId);
    return host.HandleAsync(message, cancellationToken);
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

internal class StandardActorProxyFactory : ProxyFactory<IStandardActorHost, string, IStandardActor, StandardActorState>, IStandardActorFactory
{
  public StandardActorProxyFactory(IActorProxyFactory actorFactory)
    : base(actorFactory)
  {
  }

  protected override string ActorName => nameof(StandardActor);

  public Task<IStandardActor> CreateAsync(string id, string state1, StandardActorStateComponent state2, CancellationToken cancellationToken = default)
  {
    return CreateAsync(id, new StandardActorState { _0 = state1, _1 = state2 }, cancellationToken);
  }

  public IStandardActor Get(string id)
  {
    return GetCore(id);
  }

  protected override IStandardActor CreateProxy(IStandardActorHost actorHost)
  {
    return new StandardActorProxy(actorHost);
  }

  protected override Task InitAsync(IStandardActorHost actorHost, byte[] payload, CancellationToken cancellationToken)
  {
    return actorHost._InitAsync(payload, cancellationToken);
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
      .Setup(x => x.GetHostMethod(descriptor.Methods.ElementAt(0)))
      .Returns(HostTaskMethod);
    hostEmitResultMock
      .Setup(x => x.GetHostMethod(descriptor.Methods.ElementAt(1)))
      .Returns(HostTaskTMethod);
    hostEmitResultMock
      .Setup(x => x.GetHostMethod(descriptor.Methods.ElementAt(2)))
      .Returns(HostValueTaskMethod);
    hostEmitResultMock
      .Setup(x => x.GetHostMethod(descriptor.Methods.ElementAt(3)))
      .Returns(HostValueTaskTMethod);

    HostEmitResult = hostEmitResultMock.Object;
  }
}