using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Actors.TestModel;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

public class ReflectionActorDescriptorFactoryTests
{
  private readonly Mock<IStateTypeBuilder> _stateTypeBuilderMock;
  private readonly Mock<IActivityTypeBuilder> _activityTypeBuilderMock;
  private readonly Mock<IReflectionMetadataContext> _contextMock;

  public ReflectionActorDescriptorFactoryTests()
  {
    _stateTypeBuilderMock = new(MockBehavior.Strict);
    _activityTypeBuilderMock = new(MockBehavior.Strict);
    _contextMock = new(MockBehavior.Strict);
  }

  [Fact]
  public void CreateDescriptor_ShouldCreateCorrectDescriptor_ForStandardActor()
  {
    // Arrange
    StandardActorFixture.SetupMocks(_stateTypeBuilderMock, _activityTypeBuilderMock);

    _contextMock
      .Setup(x => x.GetFactoryType(typeof(StandardActor)))
      .Returns(typeof(IStandardActorFactory));
    _contextMock
      .Setup(x => x.GetImplementationTypes(typeof(StandardActor)))
      .Returns(Enumerable.Empty<Type>());

    ReflectionActorDescriptorFactory<StandardActor> sut = new(
      _stateTypeBuilderMock.Object,
      _activityTypeBuilderMock.Object,
      _contextMock.Object,
      new ActorAttribute<IStandardActor>(),
      null);

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

    _contextMock
      .Setup(x => x.GetFactoryType(typeof(PolymorphicActor)))
      .Returns(typeof(IPolymorphicActorFactory));
    _contextMock
      .Setup(x => x.GetImplementationTypes(typeof(PolymorphicActor)))
      .Returns(new[] { typeof(PolymorphicActorImplementation1), typeof(PolymorphicActorImplementation2) });

    ReflectionActorDescriptorFactory<PolymorphicActor> sut = new(
      _stateTypeBuilderMock.Object,
      _activityTypeBuilderMock.Object,
      _contextMock.Object,
      new ActorAttribute(),
      null);

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

    _contextMock
      .Setup(x => x.GetFactoryType(typeof(StatelessActor)))
      .Returns(typeof(IStatelessActorFactory));
    _contextMock
      .Setup(x => x.GetImplementationTypes(typeof(StatelessActor)))
      .Returns(Enumerable.Empty<Type>());

    ReflectionActorDescriptorFactory<StatelessActor> sut = new(
      _stateTypeBuilderMock.Object,
      _activityTypeBuilderMock.Object,
      _contextMock.Object,
      new ActorAttribute(),
      null);

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

    _contextMock
      .Setup(x => x.GetFactoryType(typeof(VirtualActor)))
      .Returns(typeof(IVirtualActorFactory));
    _contextMock
      .Setup(x => x.GetImplementationTypes(typeof(VirtualActor)))
      .Returns(Enumerable.Empty<Type>());

    ReflectionActorDescriptorFactory<VirtualActor> sut = new(
      _stateTypeBuilderMock.Object,
      _activityTypeBuilderMock.Object,
      _contextMock.Object,
      new ActorAttribute(),
      null);

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

    _contextMock
      .Setup(x => x.GetFactoryType(typeof(ComponentIdSourceActor)))
      .Returns(typeof(IComponentIdSourceActorFactory));
    _contextMock
      .Setup(x => x.GetImplementationTypes(typeof(ComponentIdSourceActor)))
      .Returns(Enumerable.Empty<Type>());

    ReflectionActorDescriptorFactory<ComponentIdSourceActor> sut = new(
      _stateTypeBuilderMock.Object,
      _activityTypeBuilderMock.Object,
      _contextMock.Object,
      new ActorAttribute(),
      null);

    // Act
    IActorDescriptor descriptor = sut.CreateDescriptor();

    // Assert
    ComponentIdSourceActorFixture.AssertValidDescriptor(descriptor);
  }
}
