using FluentAssertions;
using System.Net;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore.IntegrationTests;

public class NoResponseTests
{
  [Theory]
  [InlineData("by-reflection")]
  [InlineData("by-configuration")]
  public async Task SubscribedMicroservice_ShouldReciveMessage_WhenHandlerHasNoResponse(string topic)
  {
    // Arrange
    Guid messageId = Guid.NewGuid();
    int millisecondsTimeout = 10000;
    using HttpClient http = new HttpClient();

    // Act
    Task<HttpResponseMessage> waitTask = http.GetAsync($"http://localhost:50001/noresponse/wait/{messageId}?millisecondsTimeout={millisecondsTimeout}");
    HttpResponseMessage sendResponse = await http.GetAsync($"http://localhost:50000/noresponse/{topic}/send/{messageId}");
    HttpResponseMessage waitResponse = await waitTask;

    // Assert
    sendResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    waitResponse.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
