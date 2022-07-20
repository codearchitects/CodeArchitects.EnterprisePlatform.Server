using FluentAssertions;

namespace CodeArchitects.Platform.Application.SignalR;

public class HubEndpointAttributeTests
{
  [Fact]
  public void Constructor_ShouldSetPattern()
  {
    // Arrange
    const string endpoint = nameof(endpoint);

    // Act
    HubEndpointAttribute attribute = new HubEndpointAttribute(endpoint);

    // Assert
    attribute.Endpoint.Should().Be(endpoint);
  }
}
