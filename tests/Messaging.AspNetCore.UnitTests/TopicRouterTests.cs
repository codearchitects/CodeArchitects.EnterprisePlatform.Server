using CodeArchitects.Platform.Messaging.AspNetCore.Fixtures;
using CodeArchitects.Platform.Messaging.AspNetCore.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using static CodeArchitects.Platform.Messaging.AspNetCore.TopicRouterFixture;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

public class TopicRouterTests
{
  private readonly Mock<HandlerDelegate> _delegateMock;
  private readonly Mock<IDictionary<string, HandlerDelegate>> _delegatesMock;
  private readonly Mock<IMessageBiMap> _messageMapMock;
  private readonly Mock<HttpContext> _contextMock;

  private readonly TopicRouter _sut;

  public TopicRouterTests()
  {
    _delegateMock = new(MockBehavior.Strict);
    _messageMapMock = new(MockBehavior.Strict);
    _contextMock = new(MockBehavior.Strict);

    _contextMock
      .Setup(x => x.RequestServices.GetService(typeof(ILoggerFactory)))
      .Returns(null);
    _contextMock
      .Setup(x => x.RequestAborted)
      .Returns(CancellationToken.None);
    _contextMock
      .SetupProperty(x => x.Response.StatusCode);

    HandlerDelegate? @delegate = _delegateMock.Object;

    _delegatesMock = new(MockBehavior.Strict);
    _delegatesMock
      .Setup(x => x.Add(It.IsAny<string>(), It.IsAny<HandlerDelegate>()));
    _delegatesMock
      .Setup(x => x.TryGetValue(It.IsAny<string>(), out @delegate))
      .Returns(true);

    _sut = new(_delegatesMock.Object, _messageMapMock.Object);
  }

  [Theory]
  [RequestBodyData(JsonMessages.GoodMessageRawPayload, "application/json", StatusCodes.Status200OK)]
  [RequestBodyData(JsonMessages.GoodMessageCloudEvent, "application/cloudevents+json", StatusCodes.Status200OK)]
  public async Task ExecuteAsync_ShouldRespondWith200_WhenMessageIsWellFormed(Stream body, string contentType, int statusCode)
  {
    // Arrange
    HttpContext context = _contextMock.Object;

    _delegateMock
      .Setup(x => x.HandleAsync(context, It.IsAny<JObject>()))
      .Returns(Task.CompletedTask);

    _contextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _contextMock
      .Setup(x => x.Request.ContentType)
      .Returns(contentType);

    // Act
    await _sut.ExecuteAsync(context);

    // Assert
    context.Response.StatusCode.Should().Be(statusCode);
    _delegateMock.Verify(x => x.HandleAsync(context, It.IsAny<JObject>()));

    // Teardown
    body.Dispose();
  }

  [Theory]
  [RequestBodyData("", "any", StatusCodes.Status415UnsupportedMediaType)]
  [RequestBodyData("string", "application/json", StatusCodes.Status400BadRequest)]
  [RequestBodyData("string", "application/cloudevents+json", StatusCodes.Status400BadRequest)]
  [RequestBodyData(JsonMessages.BadMessageRawPayload1, "application/json", StatusCodes.Status400BadRequest)]
  [RequestBodyData(JsonMessages.BadMessageRawPayload2, "application/json", StatusCodes.Status400BadRequest)]
  [RequestBodyData(JsonMessages.BadMessageCloudEvent1, "application/cloudevents+json", StatusCodes.Status400BadRequest)]
  [RequestBodyData(JsonMessages.BadMessageCloudEvent2, "application/cloudevents+json", StatusCodes.Status400BadRequest)]
  [RequestBodyData(JsonMessages.BadMessageCloudEvent3, "application/cloudevents+json", StatusCodes.Status400BadRequest)]
  [RequestBodyData(JsonMessages.BadMessageCloudEvent4, "application/cloudevents+json", StatusCodes.Status400BadRequest)]
  [RequestBodyData(JsonMessages.UnsupportedMessageCloudEvent, "application/cloudevents+json", StatusCodes.Status500InternalServerError)]
  public async Task ExecuteAsync_ShouldNotRespondWith200_WhenMessageCannotBeHandled(Stream body, string contentType, int statusCode)
  {
    // Arrange
    HttpContext context = _contextMock.Object;

    _contextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _contextMock
      .Setup(x => x.Request.ContentType)
      .Returns(contentType);

    // Act
    await _sut.ExecuteAsync(context);

    // Assert
    context.Response.StatusCode.Should().Be(statusCode);
  }

  [Theory]
  [RequestBodyData(JsonMessages.GoodMessageRawPayload, "application/json", StatusCodes.Status400BadRequest)]
  public async Task ExecuteAsync_ShouldRespondWith400_WhenMessageTypeIsInvalid(Stream body, string contentType, int statusCode)
  {
    // Arrange
    HttpContext context = _contextMock.Object;

    _delegateMock
      .Setup(x => x.HandleAsync(context, It.IsAny<JObject>()))
      .ThrowsAsync(new InvalidMessageTypeException(typeof(Message1)));

    _contextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _contextMock
      .Setup(x => x.Request.ContentType)
      .Returns(contentType);

    // Act
    await _sut.ExecuteAsync(context);

    // Assert
    context.Response.StatusCode.Should().Be(statusCode);
    _delegateMock.Verify(x => x.HandleAsync(context, It.IsAny<JObject>()));

    // Teardown
    body.Dispose();
  }

  [Theory]
  [RequestBodyData(JsonMessages.GoodMessageRawPayload, "application/json", StatusCodes.Status500InternalServerError)]
  public async Task ExecuteAsync_ShouldRespondWith500_WhenDelegateThrowsException(Stream body, string contentType, int statusCode)
  {
    // Arrange
    HttpContext context = _contextMock.Object;

    _delegateMock
      .Setup(x => x.HandleAsync(context, It.IsAny<JObject>()))
      .ThrowsAsync(new Exception());

    _contextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _contextMock
      .Setup(x => x.Request.ContentType)
      .Returns(contentType);

    // Act
    await _sut.ExecuteAsync(context);

    // Assert
    context.Response.StatusCode.Should().Be(statusCode);
    _delegateMock.Verify(x => x.HandleAsync(context, It.IsAny<JObject>()));

    // Teardown
    body.Dispose();
  }

  [Theory]
  [RequestBodyData(JsonMessages.GoodMessageRawPayload, "application/json", StatusCodes.Status200OK)]
  public async Task ExecuteAsync_ShouldRespondWith200_WhenNoHandlerIsRegisteredAndMessageTypeIsUnknown(Stream body, string contentType, int statusCode)
  {
    // Arrange
    HttpContext context = _contextMock.Object;

    HandlerDelegate? @delegate = null;
    _delegatesMock
      .Setup(x => x.TryGetValue(It.IsAny<string>(), out @delegate))
      .Returns(false);

    Type? messageType = null;
    _messageMapMock
      .Setup(x => x.TryGetValue(It.IsAny<string>(), out messageType))
      .Returns(false);

    _contextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _contextMock
      .Setup(x => x.Request.ContentType)
      .Returns(contentType);

    // Act
    await _sut.ExecuteAsync(context);

    // Assert
    context.Response.StatusCode.Should().Be(statusCode);
    _messageMapMock.Verify(x => x.TryGetValue("MyMessage", out messageType));
    _messageMapMock.VerifyNoOtherCalls();

    // Teardown
    body.Dispose();
  }

  [Theory]
  [RequestBodyData(JsonMessages.GoodMessageRawPayload, "application/json", StatusCodes.Status200OK)]
  public async Task ExecuteAsync_ShouldRespondWith200AndAddDelegate_WhenNoHandlerIsRegisteredAndFallbackHandlerIsAvailable(Stream body, string contentType, int statusCode)
  {
    // Arrange
    string baseMessageName = "Message1";
    HttpContext context = _contextMock.Object;

    _delegateMock
      .Setup(x => x.HandleAsync(context, It.IsAny<JObject>()))
      .Returns(Task.CompletedTask);

    HandlerDelegate? delegate1 = null;
    HandlerDelegate? delegate2 = _delegateMock.Object;
    _delegatesMock
      .Setup(x => x.TryGetValue("MyMessage", out delegate1))
      .Returns(false);
    _delegatesMock
      .Setup(x => x.TryGetValue(baseMessageName, out delegate2))
      .Returns(true);

    Type? messageType = typeof(Message3);
    _messageMapMock
      .Setup(x => x.TryGetValue("MyMessage", out messageType))
      .Returns(true);
    _messageMapMock
      .Setup(x => x.TryGetValue(typeof(Message1), out baseMessageName!))
      .Returns(true);

    _contextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _contextMock
      .Setup(x => x.Request.ContentType)
      .Returns(contentType);

    // Act
    await _sut.ExecuteAsync(context);

    // Assert
    context.Response.StatusCode.Should().Be(statusCode);
    _delegatesMock.Verify(x => x.TryGetValue("MyMessage", out delegate1));
    _messageMapMock.Verify(x => x.TryGetValue("MyMessage", out messageType));
    _messageMapMock.Verify(x => x.TryGetValue(typeof(Message1), out baseMessageName!));
    _delegatesMock.Verify(x => x.TryGetValue(baseMessageName, out delegate2));
    _delegatesMock.Verify(x => x.Add("MyMessage", delegate2));
    _delegatesMock.VerifyNoOtherCalls();
    _messageMapMock.VerifyNoOtherCalls();

    // Teardown
    body.Dispose();
  }

  [Theory]
  [RequestBodyData(JsonMessages.GoodMessageRawPayload, "application/json", StatusCodes.Status200OK)]
  public async Task ExecuteAsync_ShouldRespondWith200_WhenNoHandlerIsRegisteredAndFallbackHandlerIsNotAvailable(Stream body, string contentType, int statusCode)
  {
    // Arrange
    string baseMessageName = "Object";
    HttpContext context = _contextMock.Object;

    _delegateMock
      .Setup(x => x.HandleAsync(context, It.IsAny<JObject>()))
      .Returns(Task.CompletedTask);

    HandlerDelegate? @delegate = null;
    _delegatesMock
      .Setup(x => x.TryGetValue("MyMessage", out @delegate))
      .Returns(false);
    _delegatesMock
      .Setup(x => x.TryGetValue(baseMessageName, out @delegate))
      .Returns(false);

    Type? messageType = typeof(Message1);
    _messageMapMock
      .Setup(x => x.TryGetValue("MyMessage", out messageType))
      .Returns(true);

    _messageMapMock
      .Setup(x => x.TryGetValue(typeof(object), out baseMessageName!))
      .Returns(true);

    _contextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _contextMock
      .Setup(x => x.Request.ContentType)
      .Returns(contentType);

    // Act
    await _sut.ExecuteAsync(context);

    // Assert
    context.Response.StatusCode.Should().Be(statusCode);
    _delegatesMock.Verify(x => x.TryGetValue("MyMessage", out @delegate));
    _messageMapMock.Verify(x => x.TryGetValue("MyMessage", out messageType));
    _messageMapMock.Verify(x => x.TryGetValue(typeof(object), out baseMessageName!));
    _delegatesMock.Verify(x => x.TryGetValue(baseMessageName, out @delegate));
    _delegatesMock.VerifyNoOtherCalls();
    _messageMapMock.VerifyNoOtherCalls();

    // Teardown
    body.Dispose();
  }
}
