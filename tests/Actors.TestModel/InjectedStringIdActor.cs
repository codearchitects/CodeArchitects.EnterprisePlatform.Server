using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.TestModel;

internal interface IInjectedStringIdActor
{
}

[Actor]
internal class InjectedStringIdActor : IInjectedStringIdActor
{
  [ActorId]
  private readonly string _id;

  public InjectedStringIdActor(string id)
  {
    _id = id;
  }
}

internal class InjectedStringIdActorState : OrdinaryActorState
{
  public override bool Equals(object? obj) => obj is InjectedStringIdActorState;

  public override int GetHashCode() => 0;
}

internal abstract class InjectedStringIdActorActivity : Activity<InjectedStringIdActor>
{
}

[ActorFactory(typeof(InjectedStringIdActor))]
internal interface IInjectedStringIdActorFactory
{
  IInjectedStringIdActor Get(string id);
}

internal static class InjectedStringIdActorFixture
{
  public static readonly IActorDescriptor Descriptor;

  static InjectedStringIdActorFixture()
  {
    ConstructorInfo constructor = typeof(InjectedStringIdActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });

    ParameterInfo[] constructorParameters = constructor.GetParameters();

    MethodInfo factoryGetMethod = typeof(IInjectedStringIdActorFactory).GetRequiredMethod(
      name: nameof(IInjectedStringIdActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });


    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(InjectedStringIdActor)));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IInjectedStringIdActor))
      .SetActorType(typeof(InjectedStringIdActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetIsVirtual(true)
      .SetActivityBaseType(typeof(InjectedStringIdActorActivity))
      .SetMethods()
      .SetActivities()
      .SetId(_ => _
        .SetType(typeof(string))
        .SetHasIdSource(false)
        .SetStateIndex(-1))
      .SetState(_ => _
        .SetType(typeof(InjectedStringIdActorState))
        .SetFields(Array.Empty<FieldInfo>())
        .Setup(mock => mock
          .Setup(x => x.GetDefaultValue())
          .Returns(() => new InjectedStringIdActorState())))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IInjectedStringIdActorFactory))
        .SetGetMethod(factoryGetMethod))
      .SetMessageHandlers());
  }

  public static void SetupMocks(Mock<IStateTypeBuilder> stateTypeBuilderMock, Mock<IActivityTypeBuilder> activityTypeBuilderMock)
  {
    Type actorType = typeof(InjectedStringIdActor);
    Type activityBaseType = typeof(InjectedStringIdActorActivity);

    stateTypeBuilderMock
      .Setup(x => x.Build(actorType, It.IsAny<IEnumerable<IStateComponentMetadata>>(), false))
      .Returns(Descriptor.State.Type);

    activityTypeBuilderMock
      .Setup(x => x.BuildBase(actorType))
      .Returns(activityBaseType);
  }

  public static void AssertValidDescriptor(IActorDescriptor descriptor)
  {
    descriptor.Should().BeAssignableTo<IActorDescriptor<InjectedStringIdActor, InjectedStringIdActorState>>();
    descriptor.Should().BeEquivalentTo(Descriptor, opt => opt.Using<IActorDescriptor>(ActorDescriptorEqualityComparer.Instance));
  }
}
