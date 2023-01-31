using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using CodeArchitects.Platform.Common;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Fixtures.Examples;

internal interface IComponentIdSourceActor
{
}

[Actor]
internal class ComponentIdSourceActor : IComponentIdSourceActor
{
  [State, ActorId]
  private readonly int _state;

  public ComponentIdSourceActor(int state)
  {
    _state = state;
  }
}

[ActorFactory(typeof(ComponentIdSourceActor))]
internal interface IComponentIdSourceActorFactory
{
  Task<IComponentIdSourceActor> CreateAsync(int state, CancellationToken cancellationToken = default);
  IComponentIdSourceActor Get(int id);
}

internal class ComponentIdSourceActorState
{
  public int _state { get; set; }
}

internal static class ComponentIdSourceActorFixture
{
  public static readonly IActorDescriptor Descriptor;
  public static readonly IActorMetadata Metadata;

  private static readonly FieldInfo s_stateField;
  private static readonly ConstructorInfo s_constructor;

  static ComponentIdSourceActorFixture()
  {
    s_stateField = typeof(ComponentIdSourceActor).GetRequiredField(
      name: "_state",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    s_constructor = typeof(ComponentIdSourceActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    ParameterInfo[] constructorParameters = s_constructor.GetParameters();

    MethodInfo factoryCreateAsyncMethod = typeof(IComponentIdSourceActorFactory).GetRequiredMethod(
      name: nameof(IComponentIdSourceActorFactory.CreateAsync),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(CancellationToken) });

    MethodInfo factoryGetMethod = typeof(IComponentIdSourceActorFactory).GetRequiredMethod(
      name: nameof(IComponentIdSourceActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });


    IStateDependencyDescriptor stateDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[0])
      .SetName("state")
      .SetType(typeof(int))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(s_stateField));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(ComponentIdSourceActor))
      .SetConstructor(_ => _
        .SetConstructor(s_constructor)
        .SetDependencies(stateDependency)
        .SetContextDependency(null as IContextDependencyDescriptor)
        .SetServiceDependencies()
        .SetStateDependencies(stateDependency))
      .SetMethods());

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IComponentIdSourceActor))
      .SetActorType(typeof(ComponentIdSourceActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetId(_ => _
        .SetIdType(typeof(int))
        .SetHasIdSource(true)
        .SetStateDependency(stateDependency)
        .SetStateProperty(null))
      .SetState(_ => _
        .SetStateType(typeof(ComponentIdSourceActorState))
        .SetIsStateless(false)
        .SetIsVirtual(false)
        .SetFields(s_stateField)
        .SetDiscriminatorField(null)
        .SetDefaultValues(null as IReadOnlyList<object?>))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IComponentIdSourceActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod)));


    IImplementationMetadata baseImplementationMetadata = ImplementationMetadataBuilder.Build(_ => _
      .SetIsDefault(false)
      .SetImplementationType(typeof(ComponentIdSourceActor))
      .SetConstructor(null)
      .SetHasStateFields(true));

    Metadata = ActorMetadataBuilder.Build(_ => _
      .SetInterfaceType(typeof(IComponentIdSourceActor))
      .SetActorType(typeof(ComponentIdSourceActor))
      .SetIsExplicitVirtual(false)
      .SetFactoryType(typeof(IComponentIdSourceActorFactory))
      .SetStateFields(_ => _
        .Add(_ => _
          .SetField(s_stateField)
          .SetDefaultValue(Optional<object?>.None)
          .Setup(mock => mock
            .Setup(x => x.IsActorIdSource(out It.Ref<PropertyInfo?>.IsAny))
            .Returns(true))))
      .SetBaseImplementation(baseImplementationMetadata)
      .SetImplementations());
  }

  public static void AssertValidMetadata(IActorMetadata metadata, bool hasConstructor)
  {
    PropertyInfo? actorIdProperty = null;

    metadata.InterfaceType.Should().BeNull();
    metadata.ActorType.Should().Be<ComponentIdSourceActor>();
    metadata.IsExplicitVirtual.Should().BeFalse();
    metadata.FactoryType.Should().Be<IComponentIdSourceActorFactory>();
    metadata.StateFields.Should().HaveCount(1);

    IStateFieldMetadata stateField = metadata.StateFields.ElementAt(0);
    stateField.Field.Should().Be(s_stateField);
    stateField.DefaultValue.Should().Be(Optional<object?>.None);
    stateField.IsActorIdSource(out actorIdProperty).Should().BeTrue();
    actorIdProperty.Should().BeNull();

    metadata.BaseImplementation.IsDefault.Should().BeFalse();
    metadata.BaseImplementation.ImplementationType.Should().Be<ComponentIdSourceActor>();
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
