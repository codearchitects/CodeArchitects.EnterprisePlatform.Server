using CodeArchitects.Platform.Actors.Fixtures.Examples;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

public class ActorDescriptorFactoryTests
{
  private readonly Mock<IStateTypeBuilder> _stateTypeBuilderMock;
  private readonly ActorDescriptorFactory _sut;

  public ActorDescriptorFactoryTests()
  {
    _stateTypeBuilderMock = new(MockBehavior.Strict);
    _sut = new(_stateTypeBuilderMock.Object);
  }

  [Fact]
  public void Create_ShouldReturnCorrectDescriptor_ForStandardActor()
  {
    // Arrange
    _stateTypeBuilderMock
      .Setup(x => x.Build(typeof(StandardActor), It.IsAny<IEnumerable<FieldInfo>>()))
      .Returns(typeof(StandardActorState));

    // Act
    IActorDescriptor descriptor = _sut.Create(StandardActorFixture.Metadata);

    // Assert
    descriptor.Should().BeEquivalentTo(StandardActorFixture.Descriptor);
    _stateTypeBuilderMock.Verify(x => x.Build(typeof(StandardActor), It.Is<IEnumerable<FieldInfo>>(fields => fields.SequenceEqual(StandardActorFixture.Descriptor.State.Fields))));
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
      .Setup(x => x.Build(typeof(VirtualActor), It.IsAny<IEnumerable<FieldInfo>>()))
      .Returns(typeof(VirtualActorState));

    // Act
    IActorDescriptor descriptor = _sut.Create(VirtualActorFixture.Metadata);

    // Assert
    descriptor.Should().BeEquivalentTo(VirtualActorFixture.Descriptor);
    _stateTypeBuilderMock.Verify(x => x.Build(typeof(VirtualActor), It.Is<IEnumerable<FieldInfo>>(fields => fields.SequenceEqual(VirtualActorFixture.Descriptor.State.Fields))));
  }
}
