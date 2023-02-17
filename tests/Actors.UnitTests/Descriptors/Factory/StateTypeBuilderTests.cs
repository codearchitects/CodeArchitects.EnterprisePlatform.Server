using CodeArchitects.Platform.Actors.Descriptors;
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

    FieldInfo[] stateFields = StandardActorFixture.StateFields;
    IEnumerable<IStateComponentMetadata> stateComponents = stateFields.Select((field, index) => Mock.Of<IStateComponentMetadata>(metadata =>
      metadata.Member == field &&
      metadata.Type == field.FieldType &&
      metadata.Index == index));

    IActorDescriptor actor = StandardActorFixture.Descriptor;
    Type actorType = actor.ActorType;

    StateTypeBuilder sut = new StateTypeBuilder(_module, ilProvider);

    // Act
    Type stateType = sut.Build(actorType, stateComponents, false);

    // Assert
    stateType.Namespace.Should().Be(actorType.Namespace);
    stateType.Name.Should().Be($"<{actorType.Name}>{StateTypeBuilder.ComponentName}");
    stateType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Should().HaveCount(2)
      .And.ContainSingle(property =>
        property.Name == "0" &&
        property.PropertyType == stateFields[0].FieldType &&
        property.CanRead &&
        property.CanWrite)
      .And.ContainSingle(property =>
        property.Name == "1" &&
        property.PropertyType == stateFields[1].FieldType &&
        property.CanRead &&
        property.CanWrite);

    state0GetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld("<0>k__BackingField")
      .Ret());

    state0SetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld("<0>k__BackingField")
      .Ret());

    state1GetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldfld("<1>k__BackingField")
      .Ret());

    state1SetterIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldarg_1()
      .Stfld("<1>k__BackingField")
      .Ret());
  }
}
