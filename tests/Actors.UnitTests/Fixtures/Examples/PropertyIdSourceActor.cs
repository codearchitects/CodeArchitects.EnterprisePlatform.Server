using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using CodeArchitects.Platform.Common;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Fixtures.Examples;

internal interface IPropertyIdSourceActor
{
}

internal class PropertyIdSourceActorStateComponent
{
  [ActorId]
  public int Id { get; }
}

[Actor]
internal class PropertyIdSourceActor : IPropertyIdSourceActor
{
  [State]
  private readonly PropertyIdSourceActorStateComponent _state;

  public PropertyIdSourceActor(PropertyIdSourceActorStateComponent state)
  {
    _state = state;
  }
}

[ActorFactory(typeof(PropertyIdSourceActor))]
internal interface IPropertyIdSourceActorFactory
{
  Task<IPropertyIdSourceActor> CreateAsync(PropertyIdSourceActorStateComponent state, CancellationToken cancellationToken = default);
  IPropertyIdSourceActor Get(int id);
}

internal class PropertyIdSourceActorState
{
  public PropertyIdSourceActorStateComponent _state { get; set; } = default!;
}

internal static class PropertyIdSourceActorFixture
{
  public static readonly IActorDescriptor Descriptor;
  public static readonly IActorMetadata Metadata;

  private static readonly FieldInfo s_stateField;
  private static readonly ConstructorInfo s_constructor;
  private static readonly PropertyInfo s_idProperty;

  static PropertyIdSourceActorFixture()
  {
    s_stateField = typeof(PropertyIdSourceActor).GetRequiredField(
      name: "_state",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    s_idProperty = typeof(PropertyIdSourceActorStateComponent).GetRequiredProperty(
      name: nameof(PropertyIdSourceActorStateComponent.Id),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public);

    s_constructor = typeof(PropertyIdSourceActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(PropertyIdSourceActorStateComponent) });

    ParameterInfo[] constructorParameters = s_constructor.GetParameters();

    MethodInfo factoryCreateAsyncMethod = typeof(IPropertyIdSourceActorFactory).GetRequiredMethod(
      name: nameof(IPropertyIdSourceActorFactory.CreateAsync),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(PropertyIdSourceActorStateComponent), typeof(CancellationToken) });

    MethodInfo factoryGetMethod = typeof(IPropertyIdSourceActorFactory).GetRequiredMethod(
      name: nameof(IPropertyIdSourceActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });


    IStateDependencyDescriptor stateDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[0])
      .SetName("state")
      .SetType(typeof(PropertyIdSourceActorStateComponent))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(s_stateField));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(PropertyIdSourceActor))
      .SetConstructor(_ => _
        .SetConstructor(s_constructor)
        .SetDependencies(stateDependency)
        .SetContextDependency(null as IContextDependencyDescriptor)
        .SetServiceDependencies()
        .SetStateDependencies(stateDependency))
      .SetMethods());

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IPropertyIdSourceActor))
      .SetActorType(typeof(PropertyIdSourceActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetId(_ => _
        .SetIdType(typeof(int))
        .SetHasIdSource(true)
        .SetStateDependency(stateDependency)
        .SetStateProperty(s_idProperty))
      .SetState(_ => _
        .SetStateType(typeof(PropertyIdSourceActorState))
        .SetIsStateless(false)
        .SetIsVirtual(false)
        .SetFields(s_stateField)
        .SetDiscriminatorField(null)
        .SetDefaultValues(null as IReadOnlyList<object?>))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IPropertyIdSourceActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod)));


    IImplementationMetadata baseImplementationMetadata = ImplementationMetadataBuilder.Build(_ => _
      .SetIsDefault(false)
      .SetImplementationType(typeof(PropertyIdSourceActor))
      .SetConstructor(null)
      .SetHasStateFields(true));

    PropertyInfo? stateProperty = s_idProperty;
    Metadata = ActorMetadataBuilder.Build(_ => _
      .SetInterfaceType(typeof(IPropertyIdSourceActor))
      .SetActorType(typeof(PropertyIdSourceActor))
      .SetIsExplicitVirtual(false)
      .SetFactoryType(typeof(IPropertyIdSourceActorFactory))
      .SetStateFields(_ => _
        .Add(_ => _
          .SetField(s_stateField)
          .SetDefaultValue(Optional<object?>.None)
          .Setup(mock => mock
            .Setup(x => x.IsActorIdSource(out stateProperty))
            .Returns(true))))
      .SetBaseImplementation(baseImplementationMetadata)
      .SetImplementations());
  }

  public static void AssertValidMetadata(IActorMetadata metadata, bool hasConstructor)
  {
    PropertyInfo? actorIdProperty = null;

    metadata.InterfaceType.Should().BeNull();
    metadata.ActorType.Should().Be<PropertyIdSourceActor>();
    metadata.IsExplicitVirtual.Should().BeFalse();
    metadata.FactoryType.Should().Be<IPropertyIdSourceActorFactory>();
    metadata.StateFields.Should().HaveCount(1);

    IStateFieldMetadata stateField = metadata.StateFields.ElementAt(0);
    stateField.Field.Should().Be(s_stateField);
    stateField.DefaultValue.Should().Be(Optional<object?>.None);
    stateField.IsActorIdSource(out actorIdProperty).Should().BeTrue();
    actorIdProperty.Should().BeSameAs(s_idProperty);

    metadata.BaseImplementation.IsDefault.Should().BeFalse();
    metadata.BaseImplementation.ImplementationType.Should().Be<PropertyIdSourceActor>();
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
