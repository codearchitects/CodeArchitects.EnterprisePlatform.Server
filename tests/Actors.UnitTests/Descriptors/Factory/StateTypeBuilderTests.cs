using CodeArchitects.Platform.Actors.Fixtures;
using CodeArchitects.Platform.Actors.Fixtures.Examples;
using CodeArchitects.Platform.Emit.Testing;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

public class StateTypeBuilderTests
{
  private readonly ModuleBuilder _module;

  public StateTypeBuilderTests()
  {
    _module = DynamicAssembly.CreateModule();
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

    IReadOnlyList<FieldInfo> fields = StandardActorFixture.Descriptor.State.Fields;
    string state1FieldName = fields[0].Name;
    Type state1FieldType = fields[0].FieldType;
    string state2FieldName = fields[1].Name;
    Type state2FieldType = fields[1].FieldType;
    string state1BackingFieldName = $"<{state1FieldName}>k__BackingField";
    string state2BackingFieldName = $"<{state2FieldName}>k__BackingField";

    IActorDescriptor actor = StandardActorFixture.Descriptor;
    Type actorType = actor.ActorType;

    StateTypeBuilder sut = new StateTypeBuilder(_module, ilProvider);

    // Act
    Type stateType = sut.Build(actorType, fields, false);

    // Assert
    stateType.Namespace.Should().Be(actorType.Namespace);
    stateType.Name.Should().Be($"<{actorType.Name}>{StateTypeBuilder.ComponentName}");
    stateType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Should().HaveCount(2)
      .And.ContainSingle(property =>
        property.Name == state1FieldName &&
        property.PropertyType == state1FieldType &&
        property.CanRead &&
        property.CanWrite)
      .And.ContainSingle(property =>
        property.Name == state2FieldName &&
        property.PropertyType == state2FieldType &&
        property.CanRead &&
        property.CanWrite);

    state0GetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld(state1BackingFieldName)
      .Ret());

    state0SetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld(state1BackingFieldName)
      .Ret());

    state1GetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld(state2BackingFieldName)
      .Ret());

    state1SetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld(state2BackingFieldName)
      .Ret());
  }

  [Fact]
  public void Build_ShouldBuildCorrectType_WhenActorIsPolymorphic() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYGYAE9MGVgCGwApqgN6qZXZZgB2wmA+gM6EkKZmYDmxwAbkwt+QgL6VqGbAgAMzNkWJwuvUcPUSUYoA=
  {
    // Arrange
    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator stateGetterIL = ilProvider.AddGenerator();
    FakeILGenerator stateSetterIL = ilProvider.AddGenerator();
    FakeILGenerator discriminatorGetterIL = ilProvider.AddGenerator();
    FakeILGenerator discriminatorSetterIL = ilProvider.AddGenerator();

    IReadOnlyList<FieldInfo> fields = PolymorphicActorFixture.Descriptor.State.Fields;
    string stateFieldName = fields[0].Name;
    Type stateFieldType = fields[0].FieldType;
    string discriminatorFieldName = "$discriminator";
    Type discriminatorFieldType = typeof(string);
    string state1BackingFieldName = $"<{stateFieldName}>k__BackingField";
    string state2BackingFieldName = $"<{discriminatorFieldName}>k__BackingField";

    IActorDescriptor actor = PolymorphicActorFixture.Descriptor;
    Type actorType = actor.ActorType;

    StateTypeBuilder sut = new StateTypeBuilder(_module, ilProvider);

    // Act
    Type stateType = sut.Build(actorType, fields, true);

    // Assert
    stateType.Namespace.Should().Be(actorType.Namespace);
    stateType.Name.Should().Be($"<{actorType.Name}>{StateTypeBuilder.ComponentName}");
    stateType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Should().HaveCount(2)
      .And.ContainSingle(property =>
        property.Name == stateFieldName &&
        property.PropertyType == stateFieldType &&
        property.CanRead &&
        property.CanWrite)
      .And.ContainSingle(property =>
        property.Name == discriminatorFieldName &&
        property.PropertyType == discriminatorFieldType &&
        property.CanRead &&
        property.CanWrite);

    stateGetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld(state1BackingFieldName)
      .Ret());

    stateSetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld(state1BackingFieldName)
      .Ret());

    discriminatorGetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld(state2BackingFieldName)
      .Ret());

    discriminatorSetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld(state2BackingFieldName)
      .Ret());
  }
}
