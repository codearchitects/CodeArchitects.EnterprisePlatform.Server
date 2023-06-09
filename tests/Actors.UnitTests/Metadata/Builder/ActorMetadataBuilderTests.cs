using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Actors.TestModel;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

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

    sut.AsBuilder()
      .HasInterfaceType<IStandardActor>()
      .HasFactoryType<IStandardActorFactory>()
      .HasConstructor(arg => new StandardActor(
        arg.OfType<string>(),
        arg.OfType<IService1>(),
        arg.OfType<StandardActorStateComponent>(),
        arg.Context(),
        arg.OfType<IService2>()))
      .HasState("_state1")
      .HasState("_state2")
      .IsMessageHandler(typeof(ActorMessage), handler => handler
        .WithBus("bus")
        .WithTopic("topic"));

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

    sut.AsBuilder()
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

    sut.AsBuilder()
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

    sut.AsBuilder()
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
  public void CreateDescriptor_ShouldCreateCorrectDescriptor_ForStateIdActor()
  {
    // Arrange
    StateIdActorFixture.SetupMocks(_stateTypeBuilderMock, _activityTypeBuilderMock);

    ActorMetadataBuilder<StateIdActor> sut = new(_stateTypeBuilderMock.Object, _activityTypeBuilderMock.Object);

    sut.AsBuilder()
      .HasFactoryType<IStateIdActorFactory>()
      .HasState<int>("_state", state => state
        .IsActorId());

    // Act
    IActorDescriptor descriptor = sut.CreateDescriptor();

    // Assert
    StateIdActorFixture.AssertValidDescriptor(descriptor);
  }

  [Fact]
  public void CreateDescriptor_ShouldCreateCorrectDescriptor_ForInjectedStringIdActor()
  {
    // Arrange
    InjectedStringIdActorFixture.SetupMocks(_stateTypeBuilderMock, _activityTypeBuilderMock);

    ActorMetadataBuilder<InjectedStringIdActor> sut = new(_stateTypeBuilderMock.Object, _activityTypeBuilderMock.Object);

    sut.AsBuilder()
      .HasFactoryType<IInjectedStringIdActorFactory>()
      .HasId("_id");

    // Act
    IActorDescriptor descriptor = sut.CreateDescriptor();

    // Assert
    InjectedStringIdActorFixture.AssertValidDescriptor(descriptor);
  }

  [Fact]
  public void CreateDescriptor_ShouldCreateCorrectDescriptor_ForNullDefaultStateActor()
  {
    // Arrange
    NullDefaultStateActorFixture.SetupMocks(_stateTypeBuilderMock, _activityTypeBuilderMock);

    ActorMetadataBuilder<NullDefaultStateActor> sut = new(_stateTypeBuilderMock.Object, _activityTypeBuilderMock.Object);

    sut.AsBuilder()
      .IsVirtual()
      .HasFactoryType<INullDefaultStateActorFactory>()
      .HasState<string>("_state", state => state
        .HasDefaultValue(null!));

    // Act
    IActorDescriptor descriptor = sut.CreateDescriptor();

    // Assert
    NullDefaultStateActorFixture.AssertValidDescriptor(descriptor);
  }
}
