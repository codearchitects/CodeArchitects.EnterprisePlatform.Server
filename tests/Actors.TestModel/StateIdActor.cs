using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.TestModel;

internal interface IStateIdActor
{
}

[Actor]
internal class StateIdActor : IStateIdActor
{
  [State, ActorId]
  private readonly int _state;

  public StateIdActor(int state)
  {
    _state = state;
  }
}

internal class StateIdActorState : OrdinaryActorState
{
  public int _0 { get; set; }

  public override bool Equals(object? obj) => obj is StateIdActorState other && Equals(_0, other._0);

  public override int GetHashCode() => _0.GetHashCode();
}

internal abstract class StateIdActorActivity : Activity<StateIdActor>
{
}

[ActorFactory(typeof(StateIdActor))]
internal interface IStateIdActorFactory
{
  Task<IStateIdActor> CreateAsync(int state, CancellationToken cancellationToken = default);
  IStateIdActor Get(int id);
}

internal static class StateIdActorFixture
{
  public static readonly IActorDescriptor Descriptor;

  static StateIdActorFixture()
  {
    FieldInfo stateField = typeof(StateIdActor).GetRequiredField(
      name: "_state",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    ConstructorInfo constructor = typeof(StateIdActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    ParameterInfo[] constructorParameters = constructor.GetParameters();

    MethodInfo factoryCreateAsyncMethod = typeof(IStateIdActorFactory).GetRequiredMethod(
      name: nameof(IStateIdActorFactory.CreateAsync),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int), typeof(CancellationToken) });

    MethodInfo factoryGetMethod = typeof(IStateIdActorFactory).GetRequiredMethod(
      name: nameof(IStateIdActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    FieldInfo[] stateFields = typeof(StateIdActorState).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);


    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(StateIdActor)));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStateIdActor))
      .SetActorType(typeof(StateIdActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetIsVirtual(false)
      .SetActivityBaseType(typeof(StateIdActorActivity))
      .SetMethods()
      .SetActivities()
      .SetId(_ => _
        .SetType(typeof(int))
        .SetHasIdSource(true)
        .SetStateIndex(0))
      .SetState(_ => _
        .SetType(new StateTypeDelegator(typeof(StateIdActorState)))
        .SetFields(stateFields)
        .Setup(mock => mock
          .Setup(x => x.GetDefaultValue())
          .Returns(() => new StateIdActorState())))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IStateIdActorFactory))
        .SetCreateAsyncMethod(factoryCreateAsyncMethod)
        .SetGetMethod(factoryGetMethod))
      .SetMessageHandlers());
  }

  public static void SetupMocks(Mock<IStateTypeBuilder> stateTypeBuilderMock, Mock<IActivityTypeBuilder> activityTypeBuilderMock)
  {
    Type actorType = typeof(StateIdActor);
    Type activityBaseType = typeof(StateIdActorActivity);

    stateTypeBuilderMock
      .Setup(x => x.Build(actorType, It.IsAny<IEnumerable<IStateComponentMetadata>>(), false))
      .Returns(Descriptor.State.Type);

    activityTypeBuilderMock
      .Setup(x => x.BuildBase(actorType))
      .Returns(activityBaseType);
  }

  public static void AssertValidDescriptor(IActorDescriptor descriptor)
  {
    descriptor.Should().BeAssignableTo<IActorDescriptor<StateIdActor, StateIdActorState>>();
    descriptor.Should().BeEquivalentTo(Descriptor, opt => opt.Using<IActorDescriptor>(ActorDescriptorEqualityComparer.Instance));
  }
}
