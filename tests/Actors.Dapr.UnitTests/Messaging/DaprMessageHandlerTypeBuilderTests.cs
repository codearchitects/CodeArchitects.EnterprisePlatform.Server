using CodeArchitects.Platform.Actors.Dapr.Factory;
using CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Emit.Testing;
using CodeArchitects.Platform.Emit;
using System.Reflection;
using CodeArchitects.Platform.Actors.Messaging;
using CodeArchitects.Platform.Messaging;
using Dapr.Actors.Client;

namespace CodeArchitects.Platform.Actors.Dapr.Messaging;

public class DaprMessageHandlerTypeBuilderTests
{
  private readonly FakeILGeneratorProvider _ilProvider;
  private readonly DaprMessageHandlerTypeBuilder _sut;

  public DaprMessageHandlerTypeBuilderTests()
  {
    _ilProvider = new();
    _sut = new(DynamicAssembly.NewModule(), _ilProvider);
  }

  [Fact]
  public void Build_ShouldCreateFactoryType_ForStandardActor() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKqtwpxAZuQDGpTAEkAgkOAB7YgAViMgB4BPAGLDZxVZgDemAL6pU5AEYBnYMS3Y4mKdoASMq5ulzVAHnwurYml4BYVIAGkx8RzkxSgA+VExMAHdCPlFIj2IYzBBMGhlgGgBXCAhUPQTEzAAHYjAAN3JeTDIqGRoIXUlMxRUNLU9MAH1CV2B3bVVOStqC0mlSSgdMv3GBnQAKbu1etQnB0bd11QBKSoqUKuHDtczdAF5MG/2dTkTjLkuapV4FpfMrDZpKwsFFiAA5cgAW1EBjopGADCMJi+s1+vCWvjGASCghEmAAwq1eKsNhltNkwJQzl8LldEsASDIknkShAAIRvZEoD7cQJ8PGibZyACypAsFnI8J8YJi8S+iRSaQisqWuXyhTZ5UqiXJ0SWqv0mHhiO5vLQ9jBYolUtEuWFxGtkulAHEilT5XSqjAAMyYN1U5YUpZwhFIj7mngCkLiJ22pzkGiUCB8Hxx+GenXYABsmATSZTEgsqhoQjJ6dEMJt8PCBMTIlKTTA7XwMgA1qQaJhpCckYlUJH+cF8Q6K/nk3xtQqc3nExOiyWy1bxc7KyvbbX66RG8BmzRWx2uz3OOb4EHRev4eOU8QcufiKsXt5R5fSNe+OEA3FwmIx3Ob14y7VqQmbTr695/gWfBbGCuz9HcTxjE+NL0lUuRmOQFikBszzHChVReu8KJXGi8wYpgMj1HwdSUKIOCgpkkIwpg9yxJgABEIqqGC7HTGBfowLm74LqWGxAaumBVqum6ltuEBNi27adt2wD4YkhH0i+wHvreNwsYSxJvmMGxSbauCqr2WZXDAADsiFWLgwnFqJpk1iplnThGqBAA==
  {
    // Arrange
    string actorName = "name";
    IActorDescriptor actor = StandardActorFixture.Descriptor;
    Type actorType = actor.ActorType;
    Type[] handleMethodParameterTypes = new[] { typeof(ActorMessage), typeof(CancellationToken) };
    IMessageHandlerMetadata handlerMetadata = StandardActorFixture.Descriptor.MessageHandlers.ElementAt(0).HandlerMetadataCollection.ElementAt(0);

    FakeILGenerator constructorIL = _ilProvider.AddGenerator();
    FakeILGenerator actorNameGetterIL = _ilProvider.AddGenerator();
    FakeILGenerator handleAsyncIL = _ilProvider.AddGenerator();

    // Act
    Type handlerType = _sut.Build(actor, typeof(IStandardActorMessageHandler), actorName);

    // Assert
    handlerType.Namespace.Should().Be(actorType.Namespace);
    handlerType.Name.Should().Be($"<{actorType.Name}>{MessageHandlerTypeBuilder.ComponentName}");
    handlerType.Should().BeDerivedFrom<ActorHostFactory<IStandardActorMessageHandler, string>>();
    handlerType.Should().Implement<IMessageHandler<ActorMessage>>();
    handlerType.Should().BeDecoratedWith<MessageHandlerAttribute>();

    MessageHandlerAttribute attribute = handlerType.Should().HaveMethod(nameof(IMessageHandler<ActorMessage>.HandleAsync), handleMethodParameterTypes)
      .Which.Should().BeDecoratedWith<MessageHandlerAttribute>().Subject;
    attribute.Bus.Should().Be(handlerMetadata.Bus);
    attribute.Topic.Should().Be(handlerMetadata.Topic);

    constructorIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Call(typeof(ActorHostFactory<IStandardActorMessageHandler, string>), ConstructorInfo.ConstructorName, new[] { typeof(IActorProxyFactory) })
      .Ret());

    actorNameGetterIL.VerifyIL(_ => _
      .Ldstr(actorName)
      .Ret());

    handleAsyncIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Callvirt(typeof(IActorMessage<string>), $"get_{nameof(IActorMessage<string>.ActorId)}", Type.EmptyTypes)
      .Call(typeof(ActorHostFactory<IStandardActorMessageHandler, string>), "CreateHost", new[] { typeof(string) })
      .Ldarg_1()
      .Ldarg_2()
      .Callvirt(typeof(IStandardActorMessageHandler), nameof(IStandardActorMessageHandler.HandleAsync), handleMethodParameterTypes)
      .Ret());
  }
}
