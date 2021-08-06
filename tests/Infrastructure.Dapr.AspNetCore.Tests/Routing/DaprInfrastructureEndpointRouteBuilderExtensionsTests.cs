using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Routing;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Tests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using System;
using System.Text.Json;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Tests.Routing
{
  public class DaprInfrastructureEndpointRouteBuilderExtensionsTests
  {
    [Fact]
    public void CreateRequestDelegate_ShouldCreateMessageRequestDelegateOfTMessage()
    {
      // Arrange
      Type messageType = typeof(FakeMessage1);
      Type handlerType = typeof(FakeMessage1Handler1);

      // Act
      MessageRequestDelegate requestDelegate = DaprInfrastructureEndpointRouteBuilderExtensions.CreateRequestDelegate(messageType, handlerType, new JsonSerializerOptions());

      // Assert
      requestDelegate.Should().NotBeNull().And.BeOfType(typeof(MessageRequestDelegate<>).MakeGenericType(messageType));
    }
  }
}
