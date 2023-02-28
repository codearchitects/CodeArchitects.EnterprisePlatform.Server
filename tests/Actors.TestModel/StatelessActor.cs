using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.TestModel;

internal interface IStatelessActor
{
}

[Actor]
internal class StatelessActor : IStatelessActor
{
  private readonly IService1 _service1;

  public StatelessActor(IService1 service1)
  {
    _service1 = service1;
  }
}

internal class StatelessActorState : OrdinaryActorState
{
  public override bool Equals(object? obj) => obj is StatelessActorState;

  public override int GetHashCode() => 0;
}

[ActorFactory(typeof(StatelessActor))]
internal interface IStatelessActorFactory
{
  IStatelessActor Get(string id);
}

internal abstract class StatelessActorActivity : Activity<StatelessActor>
{
}

internal static class StatelessActorFixture
{
  public static readonly IActorDescriptor Descriptor;

  static StatelessActorFixture()
  {
    ConstructorInfo constructor = typeof(StatelessActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(IService1) });

    ParameterInfo[] constructorParameters = constructor.GetParameters();

    MethodInfo factoryGetMethod = typeof(IStatelessActorFactory).GetRequiredMethod(
      name: nameof(IStatelessActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(StatelessActor)));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStatelessActor))
      .SetActorType(typeof(StatelessActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetActivityBaseType(typeof(StatelessActorActivity))
      .SetIsPolymorphic(false)
      .SetIsVirtual(true)
      .SetMethods()
      .SetActivities()
      .SetId(_ => _
        .SetType(typeof(string))
        .SetHasIdSource(false)
        .SetStateIndex(-1)
        .SetGetActorIdMethod(null))
      .SetState(_ => _
        .SetType(new StateTypeDelegator(typeof(StatelessActorState)))
        .SetFields()
        .SetDefaultValue(new StatelessActorState()))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IStatelessActorFactory))
        .SetCreateAsyncMethod(null)
        .SetGetMethod(factoryGetMethod))
      .SetMessageHandlers());
  }

  public static void SetupMocks(Mock<IStateTypeBuilder> stateTypeBuilderMock, Mock<IActivityTypeBuilder> activityTypeBuilderMock)
  {
    Type actorType = typeof(StatelessActor);
    Type activityBaseType = typeof(StatelessActorActivity);

    stateTypeBuilderMock
      .Setup(x => x.Build(actorType, It.IsAny<IEnumerable<IStateComponentMetadata>>(), false))
      .Returns(Descriptor.State.Type);

    activityTypeBuilderMock
      .Setup(x => x.BuildBase(actorType))
      .Returns(activityBaseType);
  }

  public static void AssertValidDescriptor(IActorDescriptor descriptor)
  {
    descriptor.Should().BeAssignableTo<IActorDescriptor<StatelessActor, StatelessActorState>>();
    descriptor.Should().BeEquivalentTo(Descriptor, opt => opt.Using<IActorDescriptor>(ActorDescriptorEqualityComparer.Instance));
  }
}
