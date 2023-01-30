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
  internal void Build_ShouldBuildCorrectType_WhenActorHasState() // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYGYAE9MGVgCGwApqgN6qZXZZgB2wmA+gM6EkKZmYDmxwAbkwt+QgL6VqGbAgAMzNkWJwuvUcPUSUYoA=
  {
    // Arrange
    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator getter0IL = ilProvider.AddGenerator();
    FakeILGenerator setter0IL = ilProvider.AddGenerator();
    FakeILGenerator getter1IL = ilProvider.AddGenerator();
    FakeILGenerator setter1IL = ilProvider.AddGenerator();

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
    Type stateType = sut.Build(actorType, fields);

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

    getter0IL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld(state1FieldType, state1BackingFieldName)
      .Ret());

    setter0IL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld(state1FieldType, state1BackingFieldName)
      .Ret());

    getter1IL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld(state2FieldType, state2BackingFieldName)
      .Ret());

    setter1IL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld(state2FieldType, state2BackingFieldName)
      .Ret());
  }
}
