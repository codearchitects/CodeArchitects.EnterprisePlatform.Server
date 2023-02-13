using CodeArchitects.Platform.Actors.TestModel;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.Emit.Testing;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

public class StateTypeBuilderTests
{
  private readonly ModuleBuilder _module;

  public StateTypeBuilderTests()
  {
    _module = DynamicAssembly.NewModule();
  }

  [Fact]
  public void Build_ShouldBuildCorrectType_WhenActorHasState() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYGYAE9MGVgCGwApqgN6qZXZZgB2wmA+gM6EkKZmYDmxwAbkwt+QgL6VqGbAgAMzNkWJwuvUcPUSUYoA=
  {
    // Arrange
    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator state0GetterIL = ilProvider.AddGenerator();
    FakeILGenerator state0SetterIL = ilProvider.AddGenerator();
    FakeILGenerator state1GetterIL = ilProvider.AddGenerator();
    FakeILGenerator state1SetterIL = ilProvider.AddGenerator();

    IReadOnlyList<FieldInfo> fields = StandardActorFixture.Descriptor.StateFields;
    IReadOnlyList<FieldInfo> stateFields = StandardActorFixture.Descriptor.State.Fields;

    IActorDescriptor actor = StandardActorFixture.Descriptor;
    Type actorType = actor.ActorType;

    StateTypeBuilder sut = new StateTypeBuilder(_module, ilProvider);

    // Act
    Type stateType = sut.BuildOrdinary(actorType, fields);

    // Assert
    stateType.Namespace.Should().Be(actorType.Namespace);
    stateType.Name.Should().Be($"<{actorType.Name}>{StateTypeBuilder.ComponentName}");
    stateType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Should().HaveCount(2)
      .And.ContainSingle(property =>
        property.Name == fields[0].Name &&
        property.PropertyType == fields[0].FieldType &&
        property.CanRead &&
        property.CanWrite)
      .And.ContainSingle(property =>
        property.Name == fields[1].Name &&
        property.PropertyType == fields[1].FieldType &&
        property.CanRead &&
        property.CanWrite);

    state0GetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld(stateFields[0])
      .Ret());

    state0SetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld(stateFields[0])
      .Ret());

    state1GetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld(stateFields[1])
      .Ret());

    state1SetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld(stateFields[1])
      .Ret());
  }
}
