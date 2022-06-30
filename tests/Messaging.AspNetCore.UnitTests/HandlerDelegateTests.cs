using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using CodeArchitects.Platform.Messaging.AspNetCore.Fixtures;
using FluentAssertions;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

public class HandlerDelegateTests
{
  [Fact]
  public void CreateNoResult_ShouldCreateHandlerDelegateOfMessageTypeAndHandlerType()
  {
    // Arrange
    IEnumerable<OutputAction> outputActions = Enumerable.Empty<OutputAction>();

    // Act
    HandlerDelegate @delegate = HandlerDelegate.CreateNoResult(outputActions, typeof(Message1), typeof(Message1Handler));

    // Assert
    @delegate.Should().BeOfType<HandlerDelegate<Message1, Message1Handler>>();
  }

  [Fact]
  public void CreateWithResult_ShouldCreateHandlerDelegateOfMessageTypeAndResultTypeAndHandlerType()
  {
    // Arrange
    IEnumerable<OutputAction> outputActions = Enumerable.Empty<OutputAction>();

    // Act
    HandlerDelegate @delegate = HandlerDelegate.CreateWithResult(outputActions, typeof(Message2), typeof(object), typeof(Message2Handler));

    // Assert
    @delegate.Should().BeOfType<HandlerDelegate<Message2, object, Message2Handler>>();
  }
}
