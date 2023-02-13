using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Infrastructure;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.TestModel;

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

internal class ComponentIdSourceActorState : OrdinaryActorState
{
  public int _state { get; set; }
}

internal static class ComponentIdSourceActorFixture
{
  public static readonly IActorDescriptor Descriptor;

  static ComponentIdSourceActorFixture()
  {
    FieldInfo stateField = typeof(ComponentIdSourceActor).GetRequiredField(
      name: "_state",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    ConstructorInfo constructor = typeof(ComponentIdSourceActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    ParameterInfo[] constructorParameters = constructor.GetParameters();

    MethodInfo factoryCreateAsyncMethod = typeof(IComponentIdSourceActorFactory).GetRequiredMethod(
      name: nameof(IComponentIdSourceActorFactory.CreateAsync),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(CancellationToken) });

    MethodInfo factoryGetMethod = typeof(IComponentIdSourceActorFactory).GetRequiredMethod(
      name: nameof(IComponentIdSourceActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    FieldInfo[] stateFields = typeof(ComponentIdSourceActorState).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

    IStateDependencyDescriptor stateDependency = StateDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[0])
      .SetName("state")
      .SetType(typeof(int))
      .SetIndex(0)
      .SetFieldIndex(0)
      .SetField(stateField));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(ComponentIdSourceActor))
      .SetConstructor(_ => _
        .SetConstructor(constructor)
        .SetDependencies(stateDependency)
        .SetContextDependencies()
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
      .SetIsVirtual(false)
      .SetId(_ => _
        .SetType(typeof(int))
        .SetHasIdSource(true)
        .SetStateDependency(stateDependency)
        .SetIdProperty(null))
      .SetState(_ => _
        .SetType(typeof(ComponentIdSourceActorState))
        .SetFields(stateFields)
        .SetDefaultValue(null))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IComponentIdSourceActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod)));
  }
}
