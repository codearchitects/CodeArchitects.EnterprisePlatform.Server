using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Scheduling;
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

internal class PropertyIdSourceActorState : OrdinaryActorState
{
  public PropertyIdSourceActorStateComponent _0 { get; set; } = default!;
}

internal abstract class PropertyIdSourceActorActivity : Activity<PropertyIdSourceActor>
{
}

[ActorFactory(typeof(PropertyIdSourceActor))]
internal interface IPropertyIdSourceActorFactory
{
  Task<IPropertyIdSourceActor> CreateAsync(PropertyIdSourceActorStateComponent state, CancellationToken cancellationToken = default);
  IPropertyIdSourceActor Get(int id);
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


    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetId(0)
      .SetType(typeof(PropertyIdSourceActor)));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IPropertyIdSourceActor))
      .SetActorType(typeof(PropertyIdSourceActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetIsVirtual(false)
      .SetActivityBaseType(typeof(PropertyIdSourceActorActivity))
      .SetMethods()
      .SetActivities()
      .SetId(_ => _
        .SetType(typeof(int))
        .SetHasIdSource(true)
        .SetStateIndex(0)
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

  public static void SetupMocks(Mock<IStateTypeBuilder> stateTypeBuilderMock, Mock<IActivityTypeBuilder> activityTypeBuilderMock)
  {
    Type actorType = typeof(PropertyIdSourceActor);
    Type activityBaseType = typeof(PropertyIdSourceActorActivity);

    stateTypeBuilderMock
      .Setup(x => x.Build(actorType, It.IsAny<IEnumerable<IStateComponentMetadata>>(), false))
      .Returns(typeof(PropertyIdSourceActorState));

    activityTypeBuilderMock
      .Setup(x => x.BuildBase(actorType))
      .Returns(activityBaseType);
  }

  public static void AssertValidDescriptor(IActorDescriptor descriptor)
  {
    descriptor.Should().BeAssignableTo<IActorDescriptor<PropertyIdSourceActor, PropertyIdSourceActorState>>();
    descriptor.Should().BeEquivalentTo(Descriptor, opt => opt.Using<IActorDescriptor>(ActorDescriptorEqualityComparer.Instance));
  }
}
