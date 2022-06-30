using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

public class TopicDelegateTests : IClassFixture<TopicDelegateFixture>
{
  private readonly TopicDelegateFixture _fixture;

  public TopicDelegateTests(TopicDelegateFixture fixture)
  {
    _fixture = fixture;
  }

  [Fact]
  public async Task PostToTopicEndpoint_ShouldTriggerTopicDelegate()
  {
    // Arrange
    HttpContent request = JsonContent.Create(new
    {
      datacontenttype = "application/json",
      pubsubname = "messagebus",
      traceid = "00-a3946636076d5f5cb692bedba21c7166-b5c4ef5e9c24e13d-00",
      tracestate = "",
      id = "8be60316-8bab-442f-97df-ba6151a3b92f",
      specversion = "1.0",
      source = "publisher",
      type = "com.dapr.event.sent",
      topic = "__global",
      data = new
      {
        type = "Message1",
        message = new
        {
          id = "27a3e578-d9bc-412b-9a4f-1218b78fd122"
        }
      }
    });
    request.Headers.ContentType = MediaTypeHeaderValue.Parse("application/cloudevents+json");

    // Act
    HttpResponseMessage response = await _fixture.Http.PostAsync("pubsub/messagebus/__global", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
