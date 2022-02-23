using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Fakes;
using FluentAssertions;
using System;
using System.Text.Json;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Routing;

public class MessageRequestDelegateTests
{
  [Fact]
  public void Constructor_ShouldThrowInvalidHandlerConfigurationException_WhenHandlerConcreteTypeDoesNotHandleMessageType()
  {
    // Arrange

    // Act
    Func<MessageRequestDelegate> act = () => new MessageRequestDelegate<FakeMessage1>(typeof(FakeMessage2Handler), new JsonSerializerOptions());

    // Assert
    act.Should().ThrowExactly<InvalidHandlerConfigurationException>().Which.HandlerImplementationType.Should().Be(typeof(FakeMessage2Handler));
  }

  [Fact]
  public void Create_ShouldCreateMessageRequestDelegateOfTMessage()
  {
    // Arrange
    Type messageType = typeof(FakeMessage1);
    Type handlerType = typeof(FakeMessage1Handler1);

    // Act
    MessageRequestDelegate requestDelegate = MessageRequestDelegate.Create(messageType, handlerType, new JsonSerializerOptions());

    // Assert
    requestDelegate.Should().NotBeNull().And.BeOfType(typeof(MessageRequestDelegate<>).MakeGenericType(messageType));
  }
}
