using CodeArchitects.Platform.Actors.Fixtures.Examples;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

public class DescriptorFactoryTests
{
  private readonly Mock<IStateTypeBuilder> _stateTypeBuilderMock;
  private readonly DescriptorFactory _sut;

  public DescriptorFactoryTests()
  {
    _stateTypeBuilderMock = new(MockBehavior.Strict);
    _sut = new(_stateTypeBuilderMock.Object);
  }

  [Fact]
  public void Create_ShouldReturnCorrectDescriptor_ForStandardActor()
  {
    // Arrange
    _stateTypeBuilderMock
      .Setup(x => x.Build(typeof(StandardActor), It.IsAny<IEnumerable<FieldInfo>>(), false))
      .Returns(StandardActorFixture.Descriptor.State.StateType);

    // Act
    IActorDescriptor descriptor = _sut.Create(StandardActorFixture.Metadata);

    // Assert
    descriptor.Should().BeEquivalentTo(StandardActorFixture.Descriptor);
    _stateTypeBuilderMock.Verify(x => x.Build(typeof(StandardActor), It.Is<IEnumerable<FieldInfo>>(fields => fields.SequenceEqual(StandardActorFixture.Descriptor.State.Fields)), false));
  }

  [Fact]
  public void Create_ShouldReturnCorrectDescriptor_ForStatelessActor()
  {
    // Arrange

    // Act
    IActorDescriptor descriptor = _sut.Create(StatelessActorFixture.Metadata);

    // Assert
    descriptor.Should().BeEquivalentTo(StatelessActorFixture.Descriptor);
  }

  [Fact]
  public void Create_ShouldReturnCorrectDescriptor_ForVirtualActor()
  {
    // Arrange
    _stateTypeBuilderMock
      .Setup(x => x.Build(typeof(VirtualActor), It.IsAny<IEnumerable<FieldInfo>>(), false))
      .Returns(VirtualActorFixture.Descriptor.State.StateType);

    // Act
    IActorDescriptor descriptor = _sut.Create(VirtualActorFixture.Metadata);

    // Assert
    descriptor.Should().BeEquivalentTo(VirtualActorFixture.Descriptor);
    _stateTypeBuilderMock.Verify(x => x.Build(typeof(VirtualActor), It.Is<IEnumerable<FieldInfo>>(fields => fields.SequenceEqual(VirtualActorFixture.Descriptor.State.Fields)), false));
  }

  [Fact]
  public void Create_ShouldReturnCorrectDescriptor_ForComponentIdSourceActor()
  {
    // Arrange
    _stateTypeBuilderMock
      .Setup(x => x.Build(typeof(ComponentIdSourceActor), It.IsAny<IEnumerable<FieldInfo>>(), false))
      .Returns(ComponentIdSourceActorFixture.Descriptor.State.StateType);

    // Act
    IActorDescriptor descriptor = _sut.Create(ComponentIdSourceActorFixture.Metadata);

    // Assert
    descriptor.Should().BeEquivalentTo(ComponentIdSourceActorFixture.Descriptor);
    _stateTypeBuilderMock.Verify(x => x.Build(typeof(ComponentIdSourceActor), It.Is<IEnumerable<FieldInfo>>(fields => fields.SequenceEqual(ComponentIdSourceActorFixture.Descriptor.State.Fields)), false));
  }

  [Fact]
  public void Create_ShouldReturnCorrectDescriptor_ForPropertyIdSourceActor()
  {
    // Arrange
    _stateTypeBuilderMock
      .Setup(x => x.Build(typeof(PropertyIdSourceActor), It.IsAny<IEnumerable<FieldInfo>>(), false))
      .Returns(PropertyIdSourceActorFixture.Descriptor.State.StateType);

    // Act
    IActorDescriptor descriptor = _sut.Create(PropertyIdSourceActorFixture.Metadata);

    // Assert
    descriptor.Should().BeEquivalentTo(PropertyIdSourceActorFixture.Descriptor);
    _stateTypeBuilderMock.Verify(x => x.Build(typeof(PropertyIdSourceActor), It.Is<IEnumerable<FieldInfo>>(fields => fields.SequenceEqual(PropertyIdSourceActorFixture.Descriptor.State.Fields)), false));
  }

  [Fact]
  public void Create_ShouldReturnCorrectDescriptor_ForPolymorphicActor()
  {
    // Arrange
    _stateTypeBuilderMock
      .Setup(x => x.Build(typeof(PolymorphicActor), It.IsAny<IEnumerable<FieldInfo>>(), true))
      .Returns(PolymorphicActorFixture.Descriptor.State.StateType);

    // Act
    IActorDescriptor descriptor = _sut.Create(PolymorphicActorFixture.Metadata);

    // Assert
    descriptor.Should().BeEquivalentTo(PolymorphicActorFixture.Descriptor);
    _stateTypeBuilderMock.Verify(x => x.Build(typeof(PolymorphicActor), It.Is<IEnumerable<FieldInfo>>(fields => fields.SequenceEqual(PolymorphicActorFixture.Descriptor.State.Fields)), true));
  }
}
