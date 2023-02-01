using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Fixtures;
using CodeArchitects.Platform.Actors.Fixtures.Examples;
using CodeArchitects.Platform.Emit.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Infrastructure;

public class ImplementationFactoryTypeBuilderTests
{
  private readonly ModuleBuilder _module;

  public ImplementationFactoryTypeBuilderTests()
  {
    _module = DynamicAssembly.CreateModule();
  }

  [Fact]
  public void Build_ShouldBuildCorrectType_WhenActorIsOrdinary() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLAChUwMwAIwDtgCmATgGYCGAxgVgJICCFwA9kQMJP4EAewAPACoNmRAHyosWAO4ALYtUGMWWEFnioA3uNpCW7TjwE0AtgAcIBIwXxlgYDiKwAxFodPnL12/YAUASi0SEjJyWPzGZhZWwDZ2uMqqcADcqAC+qOjYeISklNT0imwchDwACkRMAG5gACbEAjqiWsFE8g3xaiiaKBL5wnrFfGFukZ6xDgDiBMD93IPh7lExPv7dgc3y8yPRXnEq8AA0oQ3JKGloKJg4nDlUtADKxFVUCFjqWGcZV9nktzQPRE8CHBXu90hdsPAsHdorhqmQiNUGtCbAR2KYOFENKkwZcssQfnlkbD4YiCljTjiIcCiXCEW0VH8YbTScJyRJLjSSQ1vDAEAAGLAAZ22BAQhz+jzAzyFkueh05dIKyMIaJMGPwQpFcHFDRmBgVLJYDgoRVm4v+gLgAH4ZQCpUCsABeLC4ACuEAgKwkXQkZw+4ISUKZXKVIrZqgh/KwAH1hSiECd2dgDUiRar1cAY3HCMDnbVyO7gABCE7+y6QlMFTYebaxRyUYQAT3DJiIYAqKKwLTI1Q4EEb91lBDKlRqxCzQ8FJy0HODiuE1cWO3rBUb3gldqoI6qtSItsBgq9rwCE83BEFTv39qnWn9SaDZGJ86UrG7hHXutNpXKO7qleExpfsA25jkQ8pzoaRDKtQ2YEEePqBDAADsLoEJID5PpB3iwQAdLGIpiqeB44ZMwAAEoEAAjq6YAtNUFr2rwG6AggIh+IcuH4Si2pYCa+jAT+oEkVMep8P+RrsUR17CcADFUExclAmxvi+ImoIUuc5YIAAbIGikgbuACiPBWIKsSCuGWm6fwWCkRR1G0QQ9FDgIbHANIYAXsx9oGeOgqTr4ToOO55ToW6HoljOkbWTapGKa53juZ5g5nr5e7+Weh5BVgIVMGF7oQJFpxAA=
  {
    // Arrange
    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator constructorIL = ilProvider.AddGenerator();
    FakeILGenerator createMethodIL = ilProvider.AddGenerator();

    IActorDescriptor actor = StandardActorFixture.Descriptor;
    Type actorType = actor.ActorType;

    ImplementationFactoryTypeBuilder sut = new ImplementationFactoryTypeBuilder(_module, ilProvider);

    // Act
    Type implementationFactoryType = sut.Build(actor);

    // Assert
    implementationFactoryType.Namespace.Should().Be(actorType.Namespace);
    implementationFactoryType.Name.Should().Be($"<{actorType.Name}>{ImplementationFactoryTypeBuilder.ComponentName}");
    implementationFactoryType.Should().Implement<IImplementationFactory<StandardActor, StandardActorState>>();
    implementationFactoryType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Should().ContainSingle()
      .Which.GetParameters().Should().HaveCount(1).And.ContainSingle(param =>
        param.Name == "services" &&
        param.ParameterType == typeof(IServiceProvider));

    constructorIL.VerifyIL(_ => _
      .Ldarg_0()
      .Call(typeof(object).GetRequiredConstructor())
      .Ldarg_0()
      .Ldarg_1()
      .Stfld("_services")
      .Ret());

    createMethodIL.VerifyNoLocals();

    createMethodIL.VerifyIL(_ => _
      .Ldarg_2()
      .Ldfld($"<{nameof(StandardActorState._state1)}>k__BackingField")
      .Ldarg_0()
      .Ldfld("_services")
      .Call(typeof(ServiceProviderServiceExtensions), nameof(ServiceProviderServiceExtensions.GetRequiredService), new[] { typeof(IService1) }, new[] { typeof(IServiceProvider) })
      .Ldarg_2()
      .Ldfld($"<{nameof(StandardActorState._state2)}>k__BackingField")
      .Ldarg_1()
      .Callvirt(typeof(IActorContextProvider<StandardActor>), nameof(IActorContextProvider<StandardActor>.GetContext), new[] { typeof(StandardActor) }, Type.EmptyTypes)
      .Ldarg_0()
      .Ldfld("_services")
      .Call(typeof(ServiceProviderServiceExtensions), nameof(ServiceProviderServiceExtensions.GetService), new[] { typeof(IService2) }, new[] { typeof(IServiceProvider) })
      .Newobj(typeof(StandardActor), new[] { typeof(string), typeof(IService1), typeof(StandardActorStateComponent), typeof(IActorContext<StandardActor>), typeof(IService2) })
      .Ret());
  }

  [Fact]
  public void Build_ShouldBuildCorrectType_WhenActorIsPolymorphic() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLAChUwMwAIwDtgCmATgGYCGAxgVgJICCFwA9kQMJP4EAewAPACoNmRAHyosWAO4ALYtUGMWWEFnioA3uNpCW7TjwE0AtgAcIBIwXxlgYDiKwAxFodPnL12/YAUASi0SEjJyWPzGZhZWwDZ2uMqqcADcqAC+qOjYeISklNT0imwchDwACkRMAG5gACbEAjqiWsFE8g3xaiiaKBL5wnrFfGFukZ6xDgDiBMD93IPh7lExPv7dgc3y8yPRXnEq8AA0oQ3JKGlo55g4nDlUtADKxFVUCFjqWGcZV9nktzQPRE8CHBXu90udPlliD88iUmBAAJ5GFgmaRgCgNDSpMGXMgAIwAzsAiJRgAksLCEUiiCi0W0VDQKYjkaj0QVMatLoyqTTWcJvDAEAAGLCEmwEQ5/R5oggvfFS54ShozAxc5m0goOChFWYrCRdCRnD4oS7wclwpnUlkNTYebaxF4qVWW9XCdkSTnm7lWgo2xY7BD8oUi7bi+7ymUi8MIRUFZV8J0863DW1LXAITXani6wIqXFkOXeUWEQ5ygHS6NYLX6YAAOmcRF4Ce9whEfmz+tBp2x2FNTZdLF9ow4wMdnrVvJYbtU2D7E6Ig7tw8DwqLoclZeekY3MpjfUz8bHzrnGerEv+gOBpYv2YkufzBELIZLUcOVYG7a0hu7ZNnSYiKZ2bBR0pccMU6LQPRAo8/wWIdcAwZdgzFM8oy3QEK16XR90bQ9Ew1St9xveI8wLVdn23Cs3x1LQOy/C4e2BX8CjuEMp3dHsgwAfVXE5AmnL4sE46owHxCgiDAIw8BsFgTiNE1GNw5sB2TP1YkcEkWHhV4AiwExxIqMUsBaMhqg4BEw23MpKhqYhBKvaV8V4wIdMgi08OEBdU3Ugp4W8ddASsqpaiINCHKIjs+Ikbjw3xLAAF5QqoRydLOPiXJnRT+xC1hjMIPylX3QKbIbJiWwI6siuCw5SpYFjDNXXxtNWSKRUkMBgAoaQsEfMUayEkSxIkqThCIviIpawIKHvLBBRAHSJsimAAHYsFwAhJDNKD3OU/9VI4ANVz6si7JimtJmAAAlAgAEcAFcwBaapz2lXh/PLVtfFfQryiC4gzqmOMcK2pT5xUuD0zbJyFokKa5SwBA5ua6G+OW1b1s2tyQc8nY4B6wgjqfE7t3xf7Lpu+7HueqhXqpmUPq+iqfuK0nAZq0Q/EOaLidJq67oeggnvDGnwzgD7fCh6HYeoOBEeRlrUbWja2ex2IEMO7jCa5wESfO3mKYF2nhYo+nyoGSq/vO1nMuPSH5om2pyFuiBQDt5HgGkcoNsV2hcAMiAagAeRMYhUwAUS4KgTB2bwABIACIaF9sh/eqLANJC4TRPEyTcGkogazj8XXc7Pi6M+AUADYyVp82iHDwhcHxWJ8TY/jK9CLBdfJ/nBe3ARW3dkSLICpngsSgh8UauKHHdz3VqdiAAEJZKAA==
  {
    // Arrange
    const string INVALID_DISCRIMINATOR = nameof(INVALID_DISCRIMINATOR);
    const string IMPLEMENTATION_1 = nameof(IMPLEMENTATION_1);
    const string IMPLEMENTATION_2 = nameof(IMPLEMENTATION_2);

    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator constructorIL = ilProvider.AddGenerator();
    FakeILGenerator createMethodIL = ilProvider.AddGenerator();

    IActorDescriptor actor = PolymorphicActorFixture.Descriptor;
    Type actorType = actor.ActorType;

    ImplementationFactoryTypeBuilder sut = new ImplementationFactoryTypeBuilder(_module, ilProvider);

    // Act
    Type implementationFactoryType = sut.Build(actor);

    // Assert
    implementationFactoryType.Namespace.Should().Be(actorType.Namespace);
    implementationFactoryType.Name.Should().Be($"<{actorType.Name}>{ImplementationFactoryTypeBuilder.ComponentName}");
    implementationFactoryType.Should().Implement<IImplementationFactory<PolymorphicActor, PolymorphicActorState>>();
    implementationFactoryType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Should().ContainSingle()
      .Which.GetParameters().Should().HaveCount(1).And.ContainSingle(param =>
        param.Name == "services" &&
        param.ParameterType == typeof(IServiceProvider));

    constructorIL.VerifyIL(_ => _
      .Ldarg_0()
      .Call(typeof(object).GetRequiredConstructor())
      .Ldarg_0()
      .Ldarg_1()
      .Stfld("_services")
      .Ret());

    createMethodIL.VerifyNoLocals();

    createMethodIL.VerifyIL(_ => _
      .Ldarg_2()
      .Ldfld($"<{nameof(PolymorphicActorState._discriminator)}>k__BackingField")
      .Switch(IMPLEMENTATION_1, IMPLEMENTATION_2)
      .Br(INVALID_DISCRIMINATOR) // On SharpLab reference we may have Br_S depending on the following instructions but we do not perform that optimization

      .MarkLabel(IMPLEMENTATION_1)
      .Ldarg_2()
      .Ldfld($"<{nameof(PolymorphicActorState._state)}>k__BackingField")
      .Ldarg_0()
      .Ldfld("_services")
      .Call(typeof(ServiceProviderServiceExtensions), nameof(ServiceProviderServiceExtensions.GetRequiredService), new[] { typeof(IService1) }, new[] { typeof(IServiceProvider) })
      .Ldarg_1()
      .Callvirt(typeof(IActorContextProvider<PolymorphicActor>), nameof(IActorContextProvider<PolymorphicActor>.GetContext), new[] { typeof(PolymorphicActorImplementation1) }, Type.EmptyTypes)
      .Newobj(typeof(PolymorphicActorImplementation1), new[] { typeof(int), typeof(IService1), typeof(IActorContext<PolymorphicActor, PolymorphicActorImplementation1>) })
      .Ret()

      .MarkLabel(IMPLEMENTATION_2)
      .Ldarg_2()
      .Ldfld($"<{nameof(PolymorphicActorState._state)}>k__BackingField")
      .Ldarg_0()
      .Ldfld("_services")
      .Call(typeof(ServiceProviderServiceExtensions), nameof(ServiceProviderServiceExtensions.GetRequiredService), new[] { typeof(IService1) }, new[] { typeof(IServiceProvider) })
      .Ldarg_1()
      .Callvirt(typeof(IActorContextProvider<PolymorphicActor>), nameof(IActorContextProvider<PolymorphicActor>.GetContext), new[] { typeof(PolymorphicActor) }, Type.EmptyTypes)
      .Ldarg_0()
      .Ldfld("_services")
      .Call(typeof(ServiceProviderServiceExtensions), nameof(ServiceProviderServiceExtensions.GetRequiredService), new[] { typeof(IService2) }, new[] { typeof(IServiceProvider) })
      .Newobj(typeof(PolymorphicActorImplementation2), new[] { typeof(int), typeof(IService1), typeof(IActorContext<PolymorphicActor>), typeof(IService2) })
      .Ret()

      .MarkLabel(INVALID_DISCRIMINATOR)
      .Ldstr("Invalid actor discriminator.")
      .Newobj(typeof(InvalidOperationException), new[] { typeof(string) })
      .Throw());
  }
}
