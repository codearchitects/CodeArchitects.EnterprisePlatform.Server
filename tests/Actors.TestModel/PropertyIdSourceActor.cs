using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Infrastructure;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.TestModel;

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

internal class PropertyIdSourceActorState : OrdinaryActorState
{
  public PropertyIdSourceActorStateComponent _state { get; set; } = default!;
}

internal static class PropertyIdSourceActorFixture
{
  public static readonly IActorDescriptor Descriptor;

  static PropertyIdSourceActorFixture()
  {
    FieldInfo stateField = typeof(PropertyIdSourceActor).GetRequiredField(
      name: "_state",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    PropertyInfo idProperty = typeof(PropertyIdSourceActorStateComponent).GetRequiredProperty(
      name: nameof(PropertyIdSourceActorStateComponent.Id),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public);

    ConstructorInfo constructor = typeof(PropertyIdSourceActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(PropertyIdSourceActorStateComponent) });

    ParameterInfo[] constructorParameters = constructor.GetParameters();

    MethodInfo factoryCreateAsyncMethod = typeof(IPropertyIdSourceActorFactory).GetRequiredMethod(
      name: nameof(IPropertyIdSourceActorFactory.CreateAsync),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(PropertyIdSourceActorStateComponent), typeof(CancellationToken) });

    MethodInfo factoryGetMethod = typeof(IPropertyIdSourceActorFactory).GetRequiredMethod(
      name: nameof(IPropertyIdSourceActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    FieldInfo[] stateFields = typeof(PropertyIdSourceActorState).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);


    IStateDependencyDescriptor stateDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[0])
      .SetName("state")
      .SetType(typeof(PropertyIdSourceActorStateComponent))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(stateField));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(PropertyIdSourceActor))
      .SetConstructor(_ => _
        .SetConstructor(constructor)
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
      .SetIsVirtual(false)
      .SetId(_ => _
        .SetType(typeof(int))
        .SetHasIdSource(true)
        .SetStateDependency(stateDependency)
        .SetIdProperty(idProperty))
      .SetState(_ => _
        .SetType(typeof(PropertyIdSourceActorState))
        .SetFields(stateFields)
        .SetDefaultValue(null))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IPropertyIdSourceActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod)));
  }
}
