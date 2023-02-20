using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Actors.TestModel;

namespace CodeArchitects.Platform.Actors.Descriptors.Builder;

public class ActorMetadataBuilderTests
{
  private readonly Mock<IStateTypeBuilder> _stateTypeBuilderMock;
  private readonly Mock<IActivityTypeBuilder> _activityTypeBuilderMock;

  public ActorMetadataBuilderTests()
  {
    _stateTypeBuilderMock = new(MockBehavior.Strict);
    _activityTypeBuilderMock = new(MockBehavior.Strict);
  }

  [Fact]
  public void CreateDescriptor_ShouldCreateCorrectDescriptor_ForStandardActor()
  {
    // Arrange
    StandardActorFixture.SetupMocks(_stateTypeBuilderMock, _activityTypeBuilderMock);

    ActorMetadataBuilder<StandardActor> sut = new(_stateTypeBuilderMock.Object, _activityTypeBuilderMock.Object);

    sut
      .HasInterfaceType<IStandardActor>()
      .HasFactoryType<IStandardActorFactory>()
      .HasConstructor(arg => new StandardActor(
        arg.OfType<string>(),
        arg.OfType<IService1>(),
        arg.OfType<StandardActorStateComponent>(),
        arg.Context(),
        arg.OfType<IService2>()))
      .HasState("_state1")
      .HasState("_state2");

    // Act
    IActorDescriptor descriptor = sut.CreateDescriptor();

    // Assert
    StandardActorFixture.AssertValidDescriptor(descriptor);
  }

  [Fact]
  public void CreateDescriptor_ShouldCreateCorrectDescriptor_ForPolymorphicActor()
  {
    // Arrange
    PolymorphicActorFixture.SetupMocks(_stateTypeBuilderMock, _activityTypeBuilderMock);

    ActorMetadataBuilder<PolymorphicActor> sut = new(_stateTypeBuilderMock.Object, _activityTypeBuilderMock.Object);

    sut
      .HasInterfaceType<IPolymorphicActor>()
      .HasFactoryType<IPolymorphicActorFactory>()
      .HasConstructor(typeof(int), typeof(IService1), typeof(IActorContext<PolymorphicActor>))
      .HasImplementation<PolymorphicActorImplementation1>(implementation => implementation
        .HasConstructor(arg => new PolymorphicActorImplementation1(
          arg.OfType<int>(),
          arg.OfType<IService1>(),
          arg.Context())))
      .HasImplementation<PolymorphicActorImplementation2>(implementation => implementation
        .IsDefault())
      .HasState("_state");

    // Act
    IActorDescriptor descriptor = sut.CreateDescriptor();

    // Assert
    PolymorphicActorFixture.AssertValidDescriptor(descriptor);
  }

  [Fact]
  public void CreateDescriptor_ShouldCreateCorrectDescriptor_ForStatelessActor()
  {
    // Arrange
    StatelessActorFixture.SetupMocks(_stateTypeBuilderMock, _activityTypeBuilderMock);

    ActorMetadataBuilder<StatelessActor> sut = new(_stateTypeBuilderMock.Object, _activityTypeBuilderMock.Object);

    sut
      .HasFactoryType<IStatelessActorFactory>();

    // Act
    IActorDescriptor descriptor = sut.CreateDescriptor();

    // Assert
    StatelessActorFixture.AssertValidDescriptor(descriptor);
  }

  [Fact]
  public void CreateDescriptor_ShouldCreateCorrectDescriptor_ForVirtualActor()
  {
    // Arrange
    VirtualActorFixture.SetupMocks(_stateTypeBuilderMock, _activityTypeBuilderMock);

    ActorMetadataBuilder<VirtualActor> sut = new(_stateTypeBuilderMock.Object, _activityTypeBuilderMock.Object);

    sut
      .HasFactoryType<IVirtualActorFactory>()
      .IsVirtual()
      .HasState("_obj")
      .HasState<string>("_state1", state => state
        .HasDefaultValue(VirtualActorFixture.State1Default))
      .HasState("_state2");

    // Act
    IActorDescriptor descriptor = sut.CreateDescriptor();

    // Assert
    VirtualActorFixture.AssertValidDescriptor(descriptor);
  }

  [Fact]
  public void CreateDescriptor_ShouldCreateCorrectDescriptor_ForComponentIdSourceActor()
  {
    // Arrange
    ComponentIdSourceActorFixture.SetupMocks(_stateTypeBuilderMock, _activityTypeBuilderMock);

    ActorMetadataBuilder<ComponentIdSourceActor> sut = new(_stateTypeBuilderMock.Object, _activityTypeBuilderMock.Object);

    sut
      .HasFactoryType<IComponentIdSourceActorFactory>()
      .HasState<int>("_state", state => state
        .IsActorId());

    // Act
    IActorDescriptor descriptor = sut.CreateDescriptor();

    // Assert
    ComponentIdSourceActorFixture.AssertValidDescriptor(descriptor);
  }

  [Fact]
  public void CreateDescriptor_ShouldCreateCorrectDescriptor_ForPropertyIdSourceActor()
  {
    // Arrange
    PropertyIdSourceActorFixture.SetupMocks(_stateTypeBuilderMock, _activityTypeBuilderMock);

    ActorMetadataBuilder<PropertyIdSourceActor> sut = new(_stateTypeBuilderMock.Object, _activityTypeBuilderMock.Object);

    sut
      .HasFactoryType<IPropertyIdSourceActorFactory>()
      .HasState("_state");

    // Act
    IActorDescriptor descriptor = sut.CreateDescriptor();

    // Assert
    PropertyIdSourceActorFixture.AssertValidDescriptor(descriptor);
  }
}
