using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Routing;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Tests.Fixtures;
using FluentAssertions;
using System;
using System.Text.Json;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Tests.Routing
{
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
  }
}
