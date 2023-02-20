using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Scheduling;
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

internal class ComponentIdSourceActorState : OrdinaryActorState
{
  public int _0 { get; set; }
}

internal abstract class ComponentIdSourceActorActivity : Activity<ComponentIdSourceActor>
{
}

[ActorFactory(typeof(ComponentIdSourceActor))]
internal interface IComponentIdSourceActorFactory
{
  Task<IComponentIdSourceActor> CreateAsync(int state, CancellationToken cancellationToken = default);
  IComponentIdSourceActor Get(int id);
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


    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(ComponentIdSourceActor)));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IComponentIdSourceActor))
      .SetActorType(typeof(ComponentIdSourceActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetIsVirtual(false)
      .SetActivityBaseType(typeof(ComponentIdSourceActorActivity))
      .SetMethods()
      .SetActivities()
      .SetId(_ => _
        .SetType(typeof(int))
        .SetHasIdSource(true)
        .SetStateIndex(0)
        .SetGetActorIdMethod(null))
      .SetState(_ => _
        .SetType(new StateTypeDelegator(typeof(ComponentIdSourceActorState)))
        .SetFields(stateFields)
        .SetDefaultValue(null))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IComponentIdSourceActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod)));
  }

  public static void SetupMocks(Mock<IStateTypeBuilder> stateTypeBuilderMock, Mock<IActivityTypeBuilder> activityTypeBuilderMock)
  {
    Type actorType = typeof(ComponentIdSourceActor);
    Type activityBaseType = typeof(ComponentIdSourceActorActivity);

    stateTypeBuilderMock
      .Setup(x => x.Build(actorType, It.IsAny<IEnumerable<IStateComponentMetadata>>(), false))
      .Returns(Descriptor.State.Type);

    activityTypeBuilderMock
      .Setup(x => x.BuildBase(actorType))
      .Returns(activityBaseType);
  }

  public static void AssertValidDescriptor(IActorDescriptor descriptor)
  {
    descriptor.Should().BeAssignableTo<IActorDescriptor<ComponentIdSourceActor, ComponentIdSourceActorState>>();
    descriptor.Should().BeEquivalentTo(Descriptor, opt => opt.Using<IActorDescriptor>(ActorDescriptorEqualityComparer.Instance));
  }
}
