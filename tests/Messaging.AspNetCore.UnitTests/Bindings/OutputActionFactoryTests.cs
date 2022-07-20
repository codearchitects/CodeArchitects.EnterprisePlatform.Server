using FluentAssertions;
using Moq;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

public class OutputActionFactoryTests
{
  private readonly OutputActionFactory _sut;

  public OutputActionFactoryTests()
  {
    _sut = new();
  }

  [Fact]
  public void Create_ShouldCreateOutputActionOfMetadataType_WhenMetadataObjectIsInstanceOfMetadataType()
  {
    // Arrange
    object @object = new();

    // Act
    OutputAction action = _sut.CreateOutputAction(typeof(object), @object, Mock.Of<IServiceProvider>());

    // Assert
    action.Should().BeOfType<OutputAction<object>>();
  }

  [Fact]
  public void Create_ShouldThrowArgumentException_WhenMetadataObjectIsNotInstanceOfMetadataType()
  {
    // Arrange
    object @object = new();

    // Act
    Func<OutputAction> act = () => _sut.CreateOutputAction(typeof(int), @object, Mock.Of<IServiceProvider>());

    // Assert
    act.Should().ThrowExactly<ArgumentException>();
  }
}
