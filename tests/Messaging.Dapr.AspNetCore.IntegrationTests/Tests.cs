using System.Net;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

public class Tests : IClassFixture<TestFixture>
{
  [Theory]
  [InlineData("by-reflection")]
  [InlineData("by-configuration")]
  public async Task SubscribedMicroservice_ShouldReciveMessage_WhenHandlerHasNoResult(string topic)
  {
    // Arrange
    Guid messageId = Guid.NewGuid();
    int millisecondsTimeout = 20000;
    using HttpClient http = new();

    // Act
    Task<HttpResponseMessage> waitTask = http.GetAsync($"http://localhost:20001/wait/{messageId}?millisecondsTimeout={millisecondsTimeout}");
    await Task.Delay(100); // To ensure the GetResult() method on the TaskCompletionSource is called after the TaskCompletionSource is created
    HttpResponseMessage sendResponse = await http.GetAsync($"http://localhost:20000/noresult/{topic}/send/{messageId}");
    HttpResponseMessage waitResponse = await waitTask;

    // Assert
    sendResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    waitResponse.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Theory]
  [InlineData("by-attribute")]
  [InlineData("by-configuration")]
  public async Task SubscribedMicroservice_ShouldReciveMessageAndInvokeBinding_WhenHandlerHasResult(string topic)
  {
    // Arrange
    Guid messageId = Guid.NewGuid();
    int millisecondsTimeout = 20000;
    using HttpClient http = new();

    // Act
    Task<HttpResponseMessage> waitTask = http.GetAsync($"http://localhost:20001/wait/{messageId}?millisecondsTimeout={millisecondsTimeout}");
    await Task.Delay(100); // To ensure the GetResult() method on the TaskCompletionSource is called after the TaskCompletionSource is created
    HttpResponseMessage sendResponse = await http.GetAsync($"http://localhost:20000/withresult/{topic}/send/{messageId}");
    HttpResponseMessage waitResponse = await waitTask;

    // Assert
    sendResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    waitResponse.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
