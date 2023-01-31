using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using CodeArchitects.Platform.Common;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Fixtures.Examples;

public interface IVirtualActor
{
}

[Actor, Virtual]
public class VirtualActor : IVirtualActor
{
  [State]
  private readonly ComplexObject _obj;
  
  [State(Default = VirtualActorFixture.State1Default)]
  private readonly string _state1;
  
  [State]
  private readonly int _state2;

  public VirtualActor(ComplexObject obj, string state1, int state2)
  {
    _obj = obj;
    _state1 = state1;
    _state2 = state2;
  }
}

public class ComplexObject
{
  public int Field0 { get; set; }
  public string Field1 { get; set; } = "field1";

  public override bool Equals(object? obj)
  {
    if (obj is not ComplexObject other)
      return false;

    return Field0 == other.Field0 && Field1 == other.Field1;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Field0, Field1);
  }
}

internal class VirtualActorState
{
  public ComplexObject _obj { get; set; } = default!;
  public string _state1 { get; set; } = default!;
  public int _state2 { get; set; }
}

[ActorFactory(typeof(VirtualActor))]
public interface IVirtualActorFactory
{
  IVirtualActor Get(string id);
}

internal static class VirtualActorFixture
{
  public const string State1Default = "state1Default";
  public static IActorDescriptor Descriptor;
  public static IActorMetadata Metadata;

  private static readonly ConstructorInfo s_constructor;
  private static readonly FieldInfo s_objField;
  private static readonly FieldInfo s_state1Field;
  private static readonly FieldInfo s_state2Field;

  static VirtualActorFixture()
  {
    s_constructor = typeof(VirtualActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      types: new[] { typeof(ComplexObject), typeof(string), typeof(int) });

    ParameterInfo[] constructorParameters = s_constructor.GetParameters();

    s_objField = typeof(VirtualActor).GetRequiredField(
      name: "_obj",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

    s_state1Field = typeof(VirtualActor).GetRequiredField(
      name: "_state1",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

    s_state2Field = typeof(VirtualActor).GetRequiredField(
      name: "_state2",
      bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

    MethodInfo factoryGetMethod = typeof(IVirtualActorFactory).GetRequiredMethod(
      name: nameof(IVirtualActorFactory.Get),
      bindingAttr: BindingFlags.Public | BindingFlags.Instance,
      types: new[] { typeof(string) });


    IStateDependencyDescriptor objDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[0])
      .SetName("obj")
      .SetType(typeof(ComplexObject))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(s_objField));

    IStateDependencyDescriptor state1Dependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[1])
      .SetName("state1")
      .SetType(typeof(string))
      .SetIndex(1)
      .SetFieldIndex(1)
      .SetField(s_state1Field));

    IStateDependencyDescriptor state2Dependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[2])
      .SetName("state2")
      .SetType(typeof(int))
      .SetIndex(2)
      .SetFieldIndex(2)
      .SetField(s_state2Field));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(VirtualActor))
      .SetConstructor(_ => _
        .SetConstructor(s_constructor)
        .SetDependencies(objDependency, state1Dependency, state2Dependency)
        .SetContextDependency(null as IContextDependencyDescriptor)
        .SetServiceDependencies()
        .SetStateDependencies(objDependency, state1Dependency, state2Dependency))
      .SetMethods());

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IVirtualActor))
      .SetActorType(typeof(VirtualActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetId(_ => _
        .SetIdType(typeof(string))
        .SetHasIdSource(false)
        .SetStateDependency(null as IStateDependencyDescriptor)
        .SetStateProperty(null))
      .SetState(_ => _
        .SetStateType(typeof(VirtualActorState))
        .SetIsStateless(false)
        .SetIsVirtual(true)
        .SetFields(s_objField, s_state1Field, s_state2Field)
        .SetDiscriminatorField(null)
        .SetDefaultValues(new ComplexObject(), State1Default, 0))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IVirtualActorFactory))
        .SetCreateAsyncMethod(null)
        .SetGetMethod(factoryGetMethod)));


    IImplementationMetadata baseImplementationMetadata = ImplementationMetadataBuilder.Build(_ => _
      .SetIsDefault(false)
      .SetImplementationType(typeof(VirtualActor))
      .SetConstructor(null)
      .SetHasStateFields(true));

    Metadata = ActorMetadataBuilder.Build(_ => _
      .SetInterfaceType(null)
      .SetActorType(typeof(VirtualActor))
      .SetIsExplicitVirtual(true)
      .SetFactoryType(typeof(IVirtualActorFactory))
      .SetStateFields(_ => _
        .Add(_ => _
          .SetField(s_objField)
          .SetDefaultValue(new ComplexObject())
          .Setup(mock => mock
            .Setup(x => x.IsActorIdSource(out It.Ref<PropertyInfo?>.IsAny))
            .Returns(false)))
        .Add(_ => _
          .SetField(s_state1Field)
          .SetDefaultValue(State1Default)
          .Setup(mock => mock
            .Setup(x => x.IsActorIdSource(out It.Ref<PropertyInfo?>.IsAny))
            .Returns(false)))
        .Add(_ => _
          .SetField(s_state2Field)
          .SetDefaultValue(0)
          .Setup(mock => mock
            .Setup(x => x.IsActorIdSource(out It.Ref<PropertyInfo?>.IsAny))
            .Returns(false))))
      .SetBaseImplementation(baseImplementationMetadata)
      .SetImplementations());
  }

  public static void AssertValidMetadata(IActorMetadata metadata, bool hasConstructor)
  {
    PropertyInfo? actorIdProperty;

    metadata.InterfaceType.Should().BeNull();
    metadata.ActorType.Should().Be<VirtualActor>();
    metadata.IsExplicitVirtual.Should().BeTrue();
    metadata.FactoryType.Should().Be<IVirtualActorFactory>();
    metadata.StateFields.Should().HaveCount(3);

    IStateFieldMetadata objField = metadata.StateFields.ElementAt(0);
    objField.Field.Should().Be(s_objField);
    objField.DefaultValue.Should().Be(Optional<object?>.None);
    objField.IsActorIdSource(out actorIdProperty).Should().BeFalse();
    actorIdProperty.Should().BeNull();

    IStateFieldMetadata state1Field = metadata.StateFields.ElementAt(1);
    state1Field.Field.Should().Be(s_state1Field);
    state1Field.DefaultValue.Should().Be(new Optional<object?>(State1Default));
    state1Field.IsActorIdSource(out actorIdProperty).Should().BeFalse();
    actorIdProperty.Should().BeNull();

    IStateFieldMetadata state2Field = metadata.StateFields.ElementAt(2);
    state2Field.Field.Should().Be(s_state2Field);
    state2Field.DefaultValue.Should().Be(Optional<object?>.None);
    state2Field.IsActorIdSource(out actorIdProperty).Should().BeFalse();
    actorIdProperty.Should().BeNull();

    metadata.BaseImplementation.IsDefault.Should().BeFalse();
    metadata.BaseImplementation.ImplementationType.Should().Be<VirtualActor>();
    metadata.BaseImplementation.HasStateFields.Should().BeTrue();
    if (hasConstructor)
    {
      metadata.BaseImplementation.Constructor.Should().BeSameAs(s_constructor);
    }
    else
    {
      metadata.BaseImplementation.Constructor.Should().BeNull();
    }

    metadata.Implementations.Should().BeEmpty();
  }
}
