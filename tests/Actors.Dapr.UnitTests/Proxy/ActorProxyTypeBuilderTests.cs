using CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;
using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.Emit.Testing;

namespace CodeArchitects.Platform.Actors.Dapr.Proxy;

public class ActorProxyTypeBuilderTests
{
  private readonly FakeILGeneratorProvider _ilProvider;
  private readonly ActorProxyTypeBuilder _sut;

  public ActorProxyTypeBuilderTests()
  {
    _ilProvider = new();
    _sut = new(DynamicAssembly.NewModule(), _ilProvider);
  }

  [Fact]
  public void Build_ShouldBuildActorProxyType_ForStandardActor() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKqtwpxN5CJh58AZuQDGpTAEkAysHI1K5YpQCCE4AHtiqAN6pM2AGymAsqWCFtlABQ9MqugEpOR0wB4eAPgtWbe0dnABpMAGElKQgIcmAwbRp8bQBrUhpMCSjSGLiEpNT0tw8PADVBAFdSGDNyiCqay2tbO0iaaNj4xOS0jKz2nM78nqLOYzLK6pNPHHQ/OoaTfCbAu2KUAF9UbhpefkFsOEwFJRU1TR1iE95MfUwtrhQRfaFn8SlZE+VVDS1dAAltABnYAGDw1fzNSgAfQQDl2TmIrncKGMNW8uz8jQCtmhcHhwERdDCbQ6eW6hT62VyXQKvXW4Nqk2xUNa1KGFN6mXZ5Lpo22qK8s3mzKWKxaDMFEOhMhoYGA6iBAE92nYvmdfpdrtIQXFSCSebSRlSBjThpT1g8dnsBEJ4MdFN9zn9iAAFYjaAAeSswIE+jo1F10YMFAAdiGAAG56zBkKiJCA++QBn5B4iAkGYaGSS4Z4BjTDggDMDtOqZd7q9SrsybLztzwMJOYBjZcHkMguM2ZdecwAF4nD3GwWrVKSxCWatgki24KO8Y0QB2LPN9ON3CT3Fw5zrYyjtHj6a+SFThGhCKG81c/pko0W9seJcrocgjcmcUw/Hnm+DXnG3f3AKB6YAsUwgaKH5sqaHJ8iat5XkUD6dtgy40KQADu4H1FMdjdg2r6gZu9g/manJFAB+7YCWhHTMKWGLMsOL2LOxjzguMCoRh9FTDMCBzLhq55rgNGMayLgUagGxAA
  {
    // Arrange
    IActorDescriptor descriptor = StandardActorFixture.Descriptor;
    Type actorType = descriptor.ActorType;

    FakeILGenerator constructorIL = _ilProvider.AddGenerator();
    FakeILGenerator taskMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator taskTMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator valueTaskMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator valueTaskTMethodIL = _ilProvider.AddGenerator();

    // Act
    Type proxyType = _sut.Build(StandardActorFixture.Descriptor, StandardActorDaprFixture.HostEmitResult);

    // Assert
    proxyType.Namespace.Should().Be(actorType.Namespace);
    proxyType.Name.Should().Be($"<{actorType.Name}>{ActorProxyTypeBuilder.ComponentName}");
    proxyType.Should().Implement<IStandardActor>();

    constructorIL.VerifyIL(_ => _
      .Ldarg_0()
      .Call(EmitUtils.ObjectConstructor)
      .Ldarg_0()
      .Ldarg_1()
      .Stfld("_actorHost")
      .Ret());

    taskMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld("_actorHost")
      .Ldarg_1()
      .Callvirt(StandardActorDaprFixture.HostTaskMethod)
      .Ret());

    taskTMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld("_actorHost")
      .Ldarg_1()
      .Ldarg_2()
      .Callvirt(StandardActorDaprFixture.HostTaskTMethod)
      .Ret());

    valueTaskMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld("_actorHost")
      .Ldarg_1()
      .Callvirt(StandardActorDaprFixture.HostValueTaskMethod)
      .Newobj(typeof(ValueTask), new[] { typeof(Task) })
      .Ret());

    valueTaskTMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld("_actorHost")
      .Callvirt(StandardActorDaprFixture.HostValueTaskTMethod)
      .Newobj(typeof(ValueTask<string>), new[] { typeof(Task<string>) })
      .Ret());
  }
}
