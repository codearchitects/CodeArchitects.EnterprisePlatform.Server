using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.TestModel;

internal interface INullDefaultStateActor
{
}

[Actor, Virtual]
internal class NullDefaultStateActor : INullDefaultStateActor
{
  [State(Default = null)] public string _state;

  public NullDefaultStateActor(string state)
  {
    _state = state;
  }
}

[ActorFactory(typeof(NullDefaultStateActor))]
internal interface INullDefaultStateActorFactory
{
  INullDefaultStateActor Get(string id);
}

internal abstract class NullDefaultStateActorActivity : Activity<NullDefaultStateActor>
{
}

internal class NullDefaultStateActorState : OrdinaryActorState
{
  public string _0 { get; set; } = default!;

  public override bool Equals(object? obj) => obj is NullDefaultStateActorState other && Equals(_0, other._0);

  public override int GetHashCode() => _0.GetHashCode();
}

internal static class NullDefaultStateActorFixture
{
  public static readonly IActorDescriptor Descriptor;

  static NullDefaultStateActorFixture()
  {
    ConstructorInfo constructor = typeof(NullDefaultStateActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });

    ParameterInfo[] constructorParameters = constructor.GetParameters();

    MethodInfo factoryGetMethod = typeof(INullDefaultStateActorFactory).GetRequiredMethod(
      name: nameof(INullDefaultStateActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });

    FieldInfo[] stateFields = typeof(NullDefaultStateActorState).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);


    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(NullDefaultStateActor)));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(INullDefaultStateActor))
      .SetActorType(typeof(NullDefaultStateActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetIsVirtual(true)
      .SetActivityBaseType(typeof(NullDefaultStateActorActivity))
      .SetMethods()
      .SetActivities()
      .SetId(_ => _
        .SetType(typeof(string))
        .SetHasIdSource(false)
        .SetStateIndex(-1))
      .SetState(_ => _
        .SetType(typeof(NullDefaultStateActorState))
        .SetFields(stateFields)
        .Setup(mock => mock
          .Setup(x => x.GetDefaultValue())
          .Returns(() => new NullDefaultStateActorState())))
      .SetFactory(_ => _
        .SetFactoryType(typeof(INullDefaultStateActorFactory))
        .SetGetMethod(factoryGetMethod))
      .SetMessageHandlers());
  }

  public static void SetupMocks(Mock<IStateTypeBuilder> stateTypeBuilderMock, Mock<IActivityTypeBuilder> activityTypeBuilderMock)
  {
    Type actorType = typeof(NullDefaultStateActor);
    Type activityBaseType = typeof(NullDefaultStateActorActivity);

    stateTypeBuilderMock
      .Setup(x => x.Build(actorType, It.IsAny<IEnumerable<IStateComponentMetadata>>(), false))
      .Returns(Descriptor.State.Type);

    activityTypeBuilderMock
      .Setup(x => x.BuildBase(actorType))
      .Returns(activityBaseType);
  }

  public static void AssertValidDescriptor(IActorDescriptor descriptor)
  {
    descriptor.Should().BeAssignableTo<IActorDescriptor<NullDefaultStateActor, NullDefaultStateActorState>>();
    descriptor.Should().BeEquivalentTo(Descriptor, opt => opt.Using<IActorDescriptor>(ActorDescriptorEqualityComparer.Instance));
  }
}

