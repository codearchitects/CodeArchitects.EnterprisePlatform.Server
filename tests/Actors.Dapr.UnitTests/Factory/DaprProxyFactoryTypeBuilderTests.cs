using CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;
using CodeArchitects.Platform.Actors.Factory;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.Emit.Testing;
using Dapr.Actors.Client;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Dapr.Factory;

public class DaprProxyFactoryTypeBuilderTests
{
  private readonly FakeILGeneratorProvider _ilProvider;
  private readonly DaprProxyFactoryTypeBuilder _sut;

  public DaprProxyFactoryTypeBuilderTests()
  {
    _ilProvider = new();
    _sut = new(DynamicAssembly.NewModule(), _ilProvider);
  }

  [Fact]
  public void Build_ShouldCreateFactoryType_ForStandardActor() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKzAzJrcKWIAzcgGNSmAJIBBUcAD2xAArF5ADwCeAMTELiGzAG9MAX1Q9M5AEYBnYMV3Y4mFeu27FGgDz4AEvLtJGgFhMVIAGkx8IJCRcUj8AGVgcgEAPlRDVExMAAdiMAA3VIkyKnkaCAMZOUVXTR1a/UwAfQ9iRr0NTmy81QE5UkoXVQb2jQAKGr169yaDds7PAEperJQcnLampeaAXksd8c4csw3MXvz5AYFh6zsHOVYsWT0AOXIAWwljOlJgBimcznK43IaWWz2RzRYKCOISADCZQEswmfgCwBicLChz0/jsyx6IP6pEGd0hj2A2AAbFIaGBgNIbBoaKI0fjMbDQuJcYoOQlkiVMHYSpEEeRWaQIBBUmAKvh5ABrUg0TCiCXiaWy+VKlWE4E5UGk240nxY7mkNKYJEUAQIxSkJks0Q+V6KSSUNJot3ED2832UAUpATC4MRa0aqUy4BymgK5Wq9WSrUxnUJ1bnHIAd0Iggk+B9fpAmBo1xoAFdpb0cnsrcASPIsyXSE23tdJJ9chBSN9YZQAKJqcS5VM0Cb6rjE67G8Ew2I4gDi/3tZFdTQ9XoL67u24zm0wObzUULw2LpeAFarmcwtcw9dUTZoLcwbcxne7vdug+Ho/HnDOFj8NiPKSIKNCUOQxCUD6RhApOMC8PAmBgRBUE+oKdryJ2FQqlSxgAWgiHOChkHQU0GGkJkvQIc8rQigICAAEJiIqtB0FoYBSpQJzYLwJFoeRYb2thT7BHRYZwMxoisfQHFcTxOTUYhCBYC09GkAgsF/FSt5qWGTEsWxckQNxwr/DeVp6SUBnSUZnEmTemDFBA5akICZw5DR/FkXoFHCbkOFiVZAjOL85m6epkmGbJ9mmTY4WWZFUkyexsWOc5rnuagBE0UBFpSN5PoclR5wwLSLRBAyTqshMhWCUK6lipGKaxvGKpqs10atbqNATgReU0OQEBOMhKTgaRPqzJgxagWNqE+YoJWGgUxQhmUlAVFUBVzRNTQcq07QckSnl8TtAkzKMkyzRK81FRi/ocnuRjVgde33Qch0YjxZw5URo03btF1uLsBjFrMINeNd43nXyGKRFDt1NJEdW+WGGQoOsJ3/dDC3KJdINTJN+PjP6INPTkxZWOQ8UTIs4xPZjcGXCSZKYPIhSCAUlASDgLxNB83wWZgABEKOKMLx28aaCOA4oVo2iU1VsrzfCBrR6kIMjZ2435WEBaJVKRU1yZdWm7VJpqptxj1jncyIlbAAzL0wAA7NayKkCujrMjVYBq0+TZi8QFGwcFGnJXZXGORrkRh1FtkxVHByRaYkQW1G2rW+m30GlLMsw8QmBLsAEwq37TvXq7RfLg6Ezlznk7LdOrPs5zfsSPnuPu7apCop3d12A9GIV/uVcB9jiNA5otNvQSDfM83Jqt8QXM87SlWMj7bL97PVKfXYWsAwXIeNRGJuZ21iadRfPUj5sVf78AuAVfSm/OhMp/py1Zu9Q3JhAA===
  {
    // Arrange
    string actorName = "name";
    IActorDescriptor actor = StandardActorFixture.Descriptor;
    Type actorType = actor.ActorType;
    IReadOnlyList<FieldInfo> stateFields = actor.State.Fields;

    FakeILGenerator createAsyncMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator getMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator constructorIL = _ilProvider.AddGenerator();
    FakeILGenerator actorNameGetterIL = _ilProvider.AddGenerator();
    FakeILGenerator createProxyMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator initAsyncMethodIL = _ilProvider.AddGenerator();

    // Act
    Type proxyFactoryType = _sut.Build(actor, StandardActorDaprFixture.HostEmitResult, typeof(StandardActorProxy), actorName);

    // Assert
    proxyFactoryType.Namespace.Should().Be(actorType.Namespace);
    proxyFactoryType.Name.Should().Be($"<{actorType.Name}>{ProxyFactoryTypeBuilder.ComponentName}");
    proxyFactoryType.Should().BeDerivedFrom<ProxyFactory<IStandardActorHost, string, IStandardActor, StandardActorState>>();
    proxyFactoryType.Should().Implement<IStandardActorFactory>();

    createAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Newobj(typeof(StandardActorState), Type.EmptyTypes)
      .Dup()
      .Ldarg_2()
      .Stfld(stateFields[0])
      .Dup()
      .Ldarg_3()
      .Stfld(stateFields[1])
      .Ldarg_S(4)
      .Call(typeof(ProxyFactory<IStandardActorHost, string, IStandardActor, StandardActorState>), "CreateCoreAsync", new[] { typeof(string), typeof(StandardActorState), typeof(CancellationToken) })
      .Ret());

    getMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Call(typeof(ProxyFactory<IStandardActorHost, string, IStandardActor, StandardActorState>), "GetCore", new[] { typeof(string) })
      .Ret());

    constructorIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Call(typeof(ProxyFactory<IStandardActorHost, string, IStandardActor, StandardActorState>), ConstructorInfo.ConstructorName, new[] { typeof(IActorProxyFactory) })
      .Ret());

    actorNameGetterIL.VerifyIL(_ => _
      .Ldstr(actorName)
      .Ret());

    createProxyMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Newobj(typeof(StandardActorProxy), new[] { typeof(IStandardActorHost) })
      .Ret());

    initAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Ldarg_2()
      .Ldarg_3()
      .Callvirt(typeof(IStandardActorHost), "_InitAsync", new[] { typeof(StandardActorState), typeof(CancellationToken) })
      .Ret());
  }

  [Fact]
  public void Build_ShouldCreateFactoryType_ForVirtualActor()
  {
    // Arrange
    string actorName = "name";
    IActorDescriptor actor = VirtualActorFixture.Descriptor;
    Type actorType = actor.ActorType;

    FakeILGenerator getMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator constructorIL = _ilProvider.AddGenerator();
    FakeILGenerator actorNameGetterIL = _ilProvider.AddGenerator();
    FakeILGenerator createProxyMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator initAsyncMethodIL = _ilProvider.AddGenerator();

    // Act
    Type proxyFactoryType = _sut.Build(actor, VirtualActorDaprFixture.HostEmitResult, typeof(VirtualActorProxy), actorName);

    // Assert
    proxyFactoryType.Namespace.Should().Be(actorType.Namespace);
    proxyFactoryType.Name.Should().Be($"<{actorType.Name}>{DaprProxyFactoryTypeBuilder.ComponentName}");
    proxyFactoryType.Should().BeDerivedFrom<ProxyFactory<IVirtualActorHost, string, IVirtualActor, VirtualActorState>>();
    proxyFactoryType.Should().Implement<IVirtualActorFactory>();

    getMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Call(typeof(ProxyFactory<IVirtualActorHost, string, IVirtualActor, VirtualActorState>), "GetCore", new[] { typeof(string) })
      .Ret());

    constructorIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Call(typeof(ProxyFactory<IVirtualActorHost, string, IVirtualActor, VirtualActorState>), ConstructorInfo.ConstructorName, new[] { typeof(IActorProxyFactory) })
      .Ret());

    actorNameGetterIL.VerifyIL(_ => _
      .Ldstr(actorName)
      .Ret());

    createProxyMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Newobj(typeof(VirtualActorProxy), new[] { typeof(IVirtualActorHost) })
      .Ret());

    initAsyncMethodIL.VerifyIL(_ => _
      .Newobj(typeof(NotSupportedException), Type.EmptyTypes)
      .Throw());
  }

  [Fact]
  public void Build_ShouldCreateFactoryType_ForComponentIdSourceActor() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKzAzJrcKWIAzcgGNSmAJIBBUcAD2xAArF5ADwCeAMTELiGzAG9MAX1Q9M5AEYBnYMV3Y4mFeu27FGgDz4AEvLtJGgFhMVIAGkx8IJCRcUj8AGVgcgEAPlRDVExMAAdiMAA3VIkyKnkaCAMZOUVXTR1a/UwAfQ9iRr0NTmy81QE5UkoXVQb2jQAKGr169yaDds7PAEperJQcnLampeaAXksd8c4csw3MXvz5AYFh6zsHOVYsWT0AOXIAWwljOlJgBimcznK43IaWWz2RzRYKCOISADCZQEswmfgCwBicLChz0/jsyx6IP6pEGd0hj2A2AAbFIaGBgNIbBoaKI0fjMbDQuJcYoOQlkiVMHYSpEEeRWaQIBBUmAKvh5ABrUg0TCiCXiaWy+VKlWE4E5UGk240nxY7mkNKYJEUAQIxSkJks0Q+V6KSSUNJot3ED2832UAUpATC4MRa0aqUy4BymgK5Wq9WSrUxnUJ1bnHIAd0Iggk+B9fpAmBo1xoAFdpb0cnsrcASPIsyXSE23tdJJ9chBSN9YZQAKJqcS5VM0Cb6rjE67G8Ew2I4gDi/3tZFdTQ9XoL67u24zm0wObzUULw2LpeAFarmcwtcw9dUTZoLcwbcxne7vdug+Ho/HnDOFj8NiPKSPanYVCqmKUIk8jlsQ4g+kYQKTjAvDwNa8jgU+wQejBcEIU0goCJkvSoXwwStCKAgAEJiIqtB0FoYBSpQJybKRvD8JRYZIX8VK3i0VGkLRoj0fQTEsYCNj/DeVqCWGIliYxzEQMMBzFBA5akICZwAWgnFcvCUhgbkEE4dBsHwY6TQciR5wwLSLRBAyTqshMJlmVBeFWT6RESEJYqRimsbxiqapBdGIW6jQE56WR6EedhXmWQRMyjAYxagZhplJbhKXWXodmGgUxQhmUlAVFUxnZZ5eX4QVfIYq07QckSORkYlkF1T5TSollWFdRZ9U+hy/ocnuRjVs1NlNQcLUYmxunAvFzideZ3mpXU6W7BlIxuDtXj9Tlg0bQ1xD8tVA3rflPpijVuVDT1eh+RkKDrO1vBrclw29dt4xTD6sw7f6O0TTkxZWOQ0kTIs4wTe9yGXCSZKYPIhSCAUlASDgLxNB83yyZgABEX3dZtxBE212BodSh2k495NWjaJSuWyXEBRGyaRWmYVJpq3NxtFN6YFjIiVsA8NTTAADs1rIqQK6Osybkc0+Tb06dvk8cY8klIpDESapwtCaYkR81G2qC+mi0GtTl3HddP16JgS7ABMXFgJQkvXjLLvLg67tezbk7FdOKNoxjnsSEdtUM2dcu2qQfUazdM12GNGLe/uvtqxhV3fU9W1uDDacS8HSNhyaEfEJj2O0s5jLK2yMcPZrpcZ3Yd352TZ1+aGoqc/zluhYmEXD9FWebL7812LgTn0o3zoTBz5vBTzMXByYQA=
  {
    // Arrange
    string actorName = "name";
    IActorDescriptor actor = ComponentIdSourceActorFixture.Descriptor;
    Type actorType = actor.ActorType;
    IReadOnlyList<FieldInfo> stateFields = actor.State.Fields;

    FakeILGenerator createAsyncMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator getMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator constructorIL = _ilProvider.AddGenerator();
    FakeILGenerator actorNameGetterIL = _ilProvider.AddGenerator();
    FakeILGenerator createProxyMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator initAsyncMethodIL = _ilProvider.AddGenerator();

    // Act
    Type proxyFactoryType = _sut.Build(actor, ComponentIdSourceActorDaprFixture.HostEmitResult, typeof(ComponentIdSourceActorProxy), actorName);

    // Assert
    proxyFactoryType.Namespace.Should().Be(actorType.Namespace);
    proxyFactoryType.Name.Should().Be($"<{actorType.Name}>{DaprProxyFactoryTypeBuilder.ComponentName}");
    proxyFactoryType.Should().BeDerivedFrom<ProxyFactory<IComponentIdSourceActorHost, int, IComponentIdSourceActor, ComponentIdSourceActorState>>();
    proxyFactoryType.Should().Implement<IComponentIdSourceActorFactory>();

    createAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Newobj(typeof(ComponentIdSourceActorState), Type.EmptyTypes)
      .Dup()
      .Ldarg_1()
      .Stfld(stateFields[0])
      .Ldarg_2()
      .Call(typeof(ProxyFactory<IComponentIdSourceActorHost, int, IComponentIdSourceActor, ComponentIdSourceActorState>), "CreateCoreAsync", new[] { typeof(int), typeof(ComponentIdSourceActorState), typeof(CancellationToken) })
      .Ret());

    getMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Call(typeof(ProxyFactory<IComponentIdSourceActorHost, int, IComponentIdSourceActor, ComponentIdSourceActorState>), "GetCore", new[] { typeof(int) })
      .Ret());

    constructorIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Call(typeof(ProxyFactory<IComponentIdSourceActorHost, int, IComponentIdSourceActor, ComponentIdSourceActorState>), ConstructorInfo.ConstructorName, new[] { typeof(IActorProxyFactory) })
      .Ret());

    actorNameGetterIL.VerifyIL(_ => _
      .Ldstr(actorName)
      .Ret());

    createProxyMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Newobj(typeof(ComponentIdSourceActorProxy), new[] { typeof(IComponentIdSourceActorHost) })
      .Ret());

    initAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Ldarg_2()
      .Ldarg_3()
      .Callvirt(typeof(IComponentIdSourceActorHost), "_InitAsync", new[] { typeof(ComponentIdSourceActorState), typeof(CancellationToken) })
      .Ret());
  }

  [Fact]
  public void Build_ShouldCreateFactoryType_ForPropertyIdSourceActor() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKzAzJrcKWIAzcgGNSmAJIBBUcAD2xAArF5ADwCeAMTELiGzAG9MAX1Q9M5AEYBnYMV3Y4mFeu27FGgDz4AEvLtJGgFhMVIAGkx8IJCRcUj8AGVgcgEAPlRDVExMAAdiMAA3VIkyKnkaCAMZOUVXTR1a/UwAfQ9iRr0NTmy81QE5UkoXVQb2jQAKGr169yaDds7PAEperJQcnLampeaAXksd8c4csw3MXvz5AYFh6zsHOVYsWT0AOXIAWwljOlJgBimcznK43IaWWz2RzRYKCOISADCZQEswmfgCwBicLChz0/jsyx6IP6pEGd0hj2A2AAbFIaGBgNIbBoaKI0fjMbDQuJcYoOQlkiVMHYSpEEeRWaQIBBUmAKvh5ABrUg0TCiCXiaWy+VKlWE4E5UGk240nxY7mkNKYJEUAQIxSkJks0Q+V6KSSUNJot3ED2832UAUpATC4MRa0aqUy4BymgK5Wq9WSrUxnUJ1bnHIAd0Iggk+B9fpAmBo1xoAFdpb0cnsrcASPIsyXSE23tdJJ9chBSN9YZQAKJqcS5VM0Cb6rjE67G8Ew2I4gDi/3tZFdTQ9XoL67u24zm0wObzUULw2LpeAFarmcwtcw9dUTZoLcwbcxne7vdug+Ho/HnDOFj8NiPKSK4uSCMAGgeok8jlsQ4g+kYQKTjAvDwCM8jgcQkHQbB8GOk0gp2vInYVCqwCZL0qF8MEUjDL8/yAmcAFoGhzhgRBUGUDBcEIYRYaUec1EcdhXE8fhPpEaQ9qkU+tEtCKAgAEJiIqtB0FoYBSpQJwXEJvAiTh3F4XxehSTJuRkfJik/JgfxUreClhipohqfQmnaYCNj/DeVpOSULluRpWkQMMBzFBA5akExqAsdRQEWlIhliSZBF4higk5DAtItEEDJOqyEzJbhvFpYoUmhqKEbJtGsbxiqaqRimdW6jQE5xWxGFYUZ4mmXUowGMWoGqN1KWlT6mV9EUQplJQFRVElI2cSVElNByrTtByRJZQZS2iStfXKANUzFcZ41rRi/ocnuRjVhtF12DeV0YrpzHAtR6Gnb1ZVHW4uyDRhYzzF4w2YctZ2reldiRKDo0HT9kRfalklhhkKDrDtXXg99PqzP9Uy4wN/3+v9N05MWVjkN5EyLOMN0Y8hlwkmSmDyIUggFJQEg4C8TQfN8vmYAAREj516EL23YGh1Ig6LkOKFaNolAVbJy4d5kkZZclUjZYpNbVaYNUmmoG3GrVPVzIiVsA9N3TAADs1rItJDoqxMNm4B6kRPk2as/RVxj+cpqnqR5oVPTZpiRMbUbamb6avQaUuLWD+0Q4dmBLsAEz8HwlC29eDuZ8uDo5/nieToazMmmzHNgFzKdw+nP1O7apCorD2PIw9VKbRiBf7kXPtY2nONNKifcEhXTPTiztfEJz3O0nljLMoVnej93UO9z3iN7T1W/lWGlUCHrNVx/Vib6xfrUD5sReT8AuC5fSq/Ou7YbR9fo6XxOpyxUAA=
  {
    // Arrange
    string actorName = "name";
    IActorDescriptor actor = PropertyIdSourceActorFixture.Descriptor;
    Type actorType = actor.ActorType;
    IReadOnlyList<FieldInfo> stateFields = actor.State.Fields;

    FakeILGenerator createAsyncMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator getMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator constructorIL = _ilProvider.AddGenerator();
    FakeILGenerator actorNameGetterIL = _ilProvider.AddGenerator();
    FakeILGenerator createProxyMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator initAsyncMethodIL = _ilProvider.AddGenerator();

    // Act
    Type proxyFactoryType = _sut.Build(actor, PropertyIdSourceActorDaprFixture.HostEmitResult, typeof(PropertyIdSourceActorProxy), actorName);

    // Assert
    proxyFactoryType.Namespace.Should().Be(actorType.Namespace);
    proxyFactoryType.Name.Should().Be($"<{actorType.Name}>{DaprProxyFactoryTypeBuilder.ComponentName}");
    proxyFactoryType.Should().BeDerivedFrom<ProxyFactory<IPropertyIdSourceActorHost, int, IPropertyIdSourceActor, PropertyIdSourceActorState>>();
    proxyFactoryType.Should().Implement<IPropertyIdSourceActorFactory>();

    createAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Callvirt(typeof(PropertyIdSourceActorStateComponent), nameof(IActorIdSource<object>.GetActorId), Type.EmptyTypes)
      .Newobj(typeof(PropertyIdSourceActorState), Type.EmptyTypes)
      .Dup()
      .Ldarg_1()
      .Stfld(stateFields[0])
      .Ldarg_2()
      .Call(typeof(ProxyFactory<IPropertyIdSourceActorHost, int, IPropertyIdSourceActor, PropertyIdSourceActorState>), "CreateCoreAsync", new[] { typeof(int), typeof(PropertyIdSourceActorState), typeof(CancellationToken) })
      .Ret());

    getMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Call(typeof(ProxyFactory<IPropertyIdSourceActorHost, int, IPropertyIdSourceActor, PropertyIdSourceActorState>), "GetCore", new[] { typeof(int) })
      .Ret());

    constructorIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Call(typeof(ProxyFactory<IPropertyIdSourceActorHost, int, IPropertyIdSourceActor, PropertyIdSourceActorState>), ConstructorInfo.ConstructorName, new[] { typeof(IActorProxyFactory) })
      .Ret());

    actorNameGetterIL.VerifyIL(_ => _
      .Ldstr(actorName)
      .Ret());

    createProxyMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Newobj(typeof(PropertyIdSourceActorProxy), new[] { typeof(IPropertyIdSourceActorHost) })
      .Ret());

    initAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_1()
      .Ldarg_2()
      .Ldarg_3()
      .Callvirt(typeof(IPropertyIdSourceActorHost), "_InitAsync", new[] { typeof(PropertyIdSourceActorState), typeof(CancellationToken) })
      .Ret());
  }
}
