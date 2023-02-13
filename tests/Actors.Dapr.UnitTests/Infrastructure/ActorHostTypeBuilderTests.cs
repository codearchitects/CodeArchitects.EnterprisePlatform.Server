using CodeArchitects.Platform.Actors.Dapr.Proxy;
using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.Emit.Testing;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using FluentAssertions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

public class ActorHostTypeBuilderTests
{
  private readonly FakeILGeneratorProvider _ilProvider;
  private readonly ActorHostTypeBuilder _sut;

  public ActorHostTypeBuilderTests()
  {
    _ilProvider = new();
    _sut = new(DynamicAssembly.NewModule(), _ilProvider);
  }

  [Fact]
  public void Build_ShouldBuildTypes_ForStandardActor() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYAYAEMEDoAqAFgE4CmAhgCZgB2A5gNyobYICsTKqtwpxN5CNjiYAysHI1K5YpQCCAY2AB7YqgDeqTNgDMmAG5hiwAK6DsANksBZUsELLKACh6YZdAJRbtmALwA+THtiZQB3TBpScIA5ZWAASQBbAAcIUkTSGl5KAFEADwVSZOAwZRonD05vGD1DYzMhGAsAHh5Aptt7Rxcst2I6ABpMAGFJQogIchKy/GUAa0zMBTHSCanSmlmFmi8UHz9A4LCIqMxYhJS0jKzSXIKi6fLK1Grao1NzADVBE1ImzG+EF+HTsDmcoxo40mjy2i2WkNW0I2sJ23m0ASCJGOkRicSSqXSmWy+UKxQ2FSqe10BneDQBPz+LRw6ECgOBFnwnTBFTRB0xIXCOLOeMuhJud1JjwpqAAvi8UDw+AJGiJxJJpLJFCpiGreJh1Jg5VwFTd+OZ4JgtaoABLKADOwH1hvlirNQldADNyIVMPErcRrJJyHQ+M18P6hvhdaRAgajdxTcrMJ7vaRffirkT1mUAGLe7UATzDEcwUYkvFjzuNrqTFoAIuRksR/baHcWlKpI9H/BpvE24qQlLdMA2my37cAnOOHZgHA6hn6OwGgyHiO3tV3yzHMIkV3wFxmxeWNnml0Xw0vN1Nt16z7ttJoqUbtH2Qrwh5RS/7LUveRijoKpznIe1zEvcZJlNKxraP277ZJYvo0GAwByHaBaQk4ZbXpgDrXkMEJQtmmzzHCKxrDCJGolS6KHFigG4hcBKgbcJIPOSzwoPGJq8G6whiBIUgyPIS6to6IAjo2zYiROzRqoJmqXvx6pCf63a9lSNRKfJwnaqJU7STOc7AAu/qBgIq6yQJGo6Z2WnWapW6BLu5n7umoqgURp6FpZykKRudkqUu3aYLehb3j44kAEbkHapBOEZQzOcGrmhaoBbhY+2hcdomn/CCXSUAA+ggPSOu4GW8jAADsP7arg+XcuVnBZfKOV6E0rRZO0Fhco4hVwKVfSDCMZFIjMlFLKNREohV1HYDV/r1T1oLdO4QzwoRFHbBxLXQdS/xsoyvVFSVBGItNE0bedW2ZLN+zVbVqi4IdDXdFd5HIpRHi4KhTRQbtryWM0zKsgyr1FQNd0+A9i0vRyx0VD9dp/TtVaA/8hXxEhKFoRhcn2UFW44Vu+FTTdNCTQiH3jdt3iZdDNVY8hqHoQoTi4bw61k5923NVWRpAA==
  {
    // Arrange
    string actorName = "name";
    IActorDescriptor descriptor = StandardActorFixture.Descriptor;
    Type actorType = descriptor.ActorType;

    Type baseType = typeof(DaprActorHost<StandardActor, StandardActorState>);

    FakeILGenerator constructorIL = _ilProvider.AddGenerator();
    FakeILGenerator taskMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator taskTMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator valueTaskMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator valueTaskTMethodIL = _ilProvider.AddGenerator();
    FakeILGenerator initAsyncMethodIL = _ilProvider.AddGenerator();

    // Act
    ActorHostEmitResult result = _sut.Build(descriptor, actorName);

    // Assert
    Type interfaceType = result.InterfaceType;
    Type classType = result.ClassType;
    var actorAttribute = classType.GetCustomAttribute<global::Dapr.Actors.Runtime.ActorAttribute>();

    interfaceType.Namespace.Should().Be(actorType.Namespace);
    interfaceType.Name.Should().Be($"<{actorType.Name}>{ActorHostTypeBuilder.InterfaceComponentName}");
    interfaceType.Should().Implement<IActor>();
    interfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Should().HaveCount(5);
    interfaceType.Should().HaveMethod($"{nameof(IStandardActor.TaskMethod)}-1", new[] { typeof(int) }).Which.Should().Return<Task>();
    interfaceType.Should().HaveMethod($"{nameof(IStandardActor.TaskMethod)}-2", new[] { typeof(int), typeof(CancellationToken) }).Which.Should().Return<Task<int>>();
    interfaceType.Should().HaveMethod(nameof(IStandardActor.ValueTaskMethod), new[] { typeof(CancellationToken) }).Which.Should().Return<ValueTask>();
    interfaceType.Should().HaveMethod(nameof(IStandardActor.ValueTaskTMethod), Type.EmptyTypes).Which.Should().Return<ValueTask<string>>();
    interfaceType.Should().HaveMethod(Constants.InitAsyncMethodName, new[] { typeof(StandardActorState), typeof(CancellationToken) }).Which.Should().Return<Task>();

    classType.Namespace.Should().Be(actorType.Namespace);
    classType.Name.Should().Be($"<{actorType.Name}>{ActorHostTypeBuilder.HostComponentName}");
    classType.Should().BeDerivedFrom(baseType);
    classType.Should().NotBeAbstract();
    classType.Should().Implement(interfaceType);
    actorAttribute.Should().NotBeNull();
    actorAttribute!.TypeName.Should().Be(actorName);

    constructorIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Ldarg_2()
      .Ldarg_3()
      .Call(baseType, ConstructorInfo.ConstructorName, new[] { typeof(ActorHost), typeof(IActorManager<StandardActor, StandardActorState>), typeof(IImplementationFactory<StandardActor, StandardActorState>) })
      .Ret());

    taskMethodIL.VerifyNoLocals();
    taskMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Call(baseType, "get_Actor", Type.EmptyTypes)
      .Ldarg_1()
      .Callvirt(actorType, nameof(StandardActor.TaskMethod), new[] { typeof(int) })
      .Ret());

    taskTMethodIL.VerifyNoLocals();
    taskTMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Call(baseType, "get_Actor", Type.EmptyTypes)
      .Ldarg_1()
      .Ldarg_2()
      .Callvirt(actorType, nameof(StandardActor.TaskMethod), new[] { typeof(int), typeof(CancellationToken) })
      .Ret());

    valueTaskMethodIL.VerifyLocals(_ => _
      .OfType(typeof(ValueTask)));
    valueTaskMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Call(baseType, "get_Actor", Type.EmptyTypes)
      .Ldarg_1()
      .Callvirt(actorType, nameof(StandardActor.ValueTaskMethod), new[] { typeof(CancellationToken) })
      .Stloc_0()
      .Ldloca_S(0)
      .Call(typeof(ValueTask), nameof(ValueTask.AsTask), Type.EmptyTypes)
      .Ret());

    valueTaskTMethodIL.VerifyLocals(_ => _
      .OfType(typeof(ValueTask<string>)));
    valueTaskTMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Call(baseType, "get_Actor", Type.EmptyTypes)
      .Callvirt(actorType, nameof(StandardActor.ValueTaskTMethod), Type.EmptyTypes)
      .Stloc_0()
      .Ldloca_S(0)
      .Call(typeof(ValueTask<string>), nameof(ValueTask<string>.AsTask), Type.EmptyTypes)
      .Ret());

    initAsyncMethodIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Ldarg_2()
      .Call(baseType, "InitAsync", new[] { typeof(StandardActorState), typeof(CancellationToken) })
      .Ret());
  }

  [Fact]
  public void Build_ShouldBuildTypes_ForVirtualActor()
  {
    // Arrange
    string actorName = "name";
    IActorDescriptor descriptor = VirtualActorFixture.Descriptor;
    Type actorType = descriptor.ActorType;

    Type baseType = typeof(DaprActorHost<VirtualActor, VirtualActorState>);

    _ = _ilProvider.AddGenerator(); // Constructor

    // Act
    ActorHostEmitResult result = _sut.Build(descriptor, actorName);

    // Assert
    Type interfaceType = result.InterfaceType;
    Type classType = result.ClassType;
    var actorAttribute = classType.GetCustomAttribute<global::Dapr.Actors.Runtime.ActorAttribute>();

    interfaceType.Namespace.Should().Be(actorType.Namespace);
    interfaceType.Name.Should().Be($"<{actorType.Name}>{ActorHostTypeBuilder.InterfaceComponentName}");
    interfaceType.Should().Implement<IActor>();
    interfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Should().BeEmpty();

    classType.Namespace.Should().Be(actorType.Namespace);
    classType.Name.Should().Be($"<{actorType.Name}>{ActorHostTypeBuilder.HostComponentName}");
    classType.Should().BeDerivedFrom(baseType);
    classType.Should().NotBeAbstract();
    classType.Should().Implement(interfaceType);
    actorAttribute.Should().NotBeNull();
    actorAttribute!.TypeName.Should().Be(actorName);
  }
}
