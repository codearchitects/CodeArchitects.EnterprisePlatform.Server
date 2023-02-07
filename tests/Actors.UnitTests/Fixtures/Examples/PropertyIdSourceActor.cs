using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
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
      .SetId(0)
      .SetType(typeof(PropertyIdSourceActor))
      .SetConstructor(_ => _
        .SetConstructor(s_constructor)
        .SetDependencies(stateDependency)
        .SetContextDependencies()
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
      .SetIsStateless(false)
      .SetIsVirtual(false)
      .SetId(_ => _
        .SetIdType(typeof(int))
        .SetHasIdSource(true)
        .SetStateDependency(stateDependency)
        .SetStateProperty(s_idProperty))
      .SetState(_ => _
        .SetType(typeof(PropertyIdSourceActorState))
        .SetStateFields(s_stateField)
        .SetDiscriminatorField(null)
        .SetDefaultValue(null))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IPropertyIdSourceActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod)));
  }
}
