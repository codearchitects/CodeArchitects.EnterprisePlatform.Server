using FluentAssertions;
using Moq;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

public class OutputActionTests
{
  [Fact]
  public void Create_ShouldCreateOutputActionOfMetadataType_WhenMetadataObjectIsInstanceOfMetadataType()
  {
    // Arrange
    object @object = new();

    // Act
    OutputAction action = OutputAction.Create(typeof(object), @object, Mock.Of<IServiceProvider>());

    // Assert
    action.Should().BeOfType<OutputAction<object>>();
  }

  [Fact]
  public void Create_ShouldThrowArgumentException_WhenMetadataObjectIsNotInstanceOfMetadataType()
  {
    // Arrange
    object @object = new();

    // Act
    Func<OutputAction> act = () => OutputAction.Create(typeof(int), @object, Mock.Of<IServiceProvider>());

    // Assert
    act.Should().ThrowExactly<ArgumentException>();
  }
}
