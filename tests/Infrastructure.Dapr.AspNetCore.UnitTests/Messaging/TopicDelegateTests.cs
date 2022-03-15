using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Fakes;
using CodeArchitects.Platform.Infrastructure.Messaging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

public class TopicDelegateTests
{
  private readonly Mock<IMessageHandler<Message1>> _handler1Mock;
  private readonly Mock<IMessageHandler<Message2, string>> _handler2Mock;
  private readonly Mock<IMessageHandler<Message3>> _handler3Mock;
  private readonly Mock<IServiceProvider> _serviceProviderMock;
  private readonly Mock<HttpContext> _httpContextMock;

  public TopicDelegateTests()
  {
    _handler1Mock = new Mock<IMessageHandler<Message1>>(MockBehavior.Strict);
    _handler1Mock
      .Setup(x => x.HandleAsync(It.IsAny<Message1>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    _handler2Mock = new Mock<IMessageHandler<Message2, string>>(MockBehavior.Strict);
    _handler2Mock
      .Setup(x => x.HandleAsync(It.IsAny<Message2>(), It.IsAny<CancellationToken>()))
      .Returns(Task.FromResult(string.Empty));

    _handler3Mock = new Mock<IMessageHandler<Message3>>(MockBehavior.Strict);
    _handler3Mock
      .Setup(x => x.HandleAsync(It.IsAny<Message3>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    Mock<ILoggerFactory> loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
    Mock<ILogger> loggerMock = new Mock<ILogger>(MockBehavior.Loose);
    loggerFactoryMock
      .Setup(x => x.CreateLogger("CAEP-Messaging"))
      .Returns(loggerMock.Object);
    _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
    _serviceProviderMock
      .Setup(x => x.GetService(Handler1.GetType()))
      .Returns(Handler1);
    _serviceProviderMock
      .Setup(x => x.GetService(Handler2.GetType()))
      .Returns(Handler2);
    _serviceProviderMock
      .Setup(x => x.GetService(Handler3.GetType()))
      .Returns(Handler3);
    _serviceProviderMock
      .Setup(x => x.GetService(typeof(ILoggerFactory)))
      .Returns(loggerFactoryMock.Object);

    _httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
    _httpContextMock
      .Setup(x => x.RequestServices)
      .Returns(ServiceProvider);
    _httpContextMock
      .SetupProperty(x => x.Response.StatusCode);
    _httpContextMock
      .Setup(x => x.RequestAborted)
      .Returns(CancellationToken.None);
  }

  private IMessageHandler<Message1> Handler1 => _handler1Mock.Object;
  private IMessageHandler<Message2, string> Handler2 => _handler2Mock.Object;
  private IMessageHandler<Message3> Handler3 => _handler3Mock.Object;
  private IServiceProvider ServiceProvider => _serviceProviderMock.Object;
  private HttpContext HttpContext => _httpContextMock.Object;
  private TopicDelegateData Handler1Data => new TopicDelegateData(typeof(Message1), null, Handler1.GetType());
  private TopicDelegateData Handler2Data => new TopicDelegateData(typeof(Message2), typeof(string), Handler2.GetType());
  private TopicDelegateData Handler3Data => new TopicDelegateData(typeof(Message3), null, Handler3.GetType());

  [Fact]
  public async Task ExecuteAsync_ShouldCallCorrectHandlerWithCorrectMessage_WhenHandlerHasNoResultAndMessageIsCloudEvent()
  {
    // Arrange
    using Stream body = CreateJsonBody(
@"{
  ""datacontenttype"": ""application/json"",
  ""pubsubname"": ""messagebus"",
	""traceid"": ""00-a3946636076d5f5cb692bedba21c7166-b5c4ef5e9c24e13d-00"",
	""tracestate"": """",
	""data"": {
    ""type"": ""Message1"",
    ""message"": {
      ""id"": ""27a3e578-d9bc-412b-9a4f-1218b78fd122""
    }
  },
	""id"": ""8be60316-8bab-442f-97df-ba6151a3b92f"",
	""specversion"": ""1.0"",
	""source"": ""publisher"",
	""type"": ""com.dapr.event.sent"",
	""topic"": ""__global""
}");
    _httpContextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _httpContextMock
      .Setup(x => x.Request.ContentType)
      .Returns("application/cloudevents+json");
    TopicDelegate sut = TopicDelegate.Create(new[] { Handler1Data }, new[] { typeof(Message1) });

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _handler1Mock.Verify(x => x.HandleAsync(It.Is<Message1>(x => x.Id.ToString() == "27a3e578-d9bc-412b-9a4f-1218b78fd122"), It.IsAny<CancellationToken>()), Times.Once);
    HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
  }

  [Fact]
  public async Task ExecuteAsync_ShouldCallCorrectHandlerWithCorrectMessage_WhenHandlerHasResultAndMessageIsCloudEvent()
  {
    // Arrange
    using Stream body = CreateJsonBody(
@"{
  ""datacontenttype"": ""application/json"",
  ""pubsubname"": ""messagebus"",
	""traceid"": ""00-a3946636076d5f5cb692bedba21c7166-b5c4ef5e9c24e13d-00"",
	""tracestate"": """",
	""data"": {
    ""type"": ""Message2"",
    ""message"": {
      ""id"": ""27a3e578-d9bc-412b-9a4f-1218b78fd122""
    }
  },
	""id"": ""8be60316-8bab-442f-97df-ba6151a3b92f"",
	""specversion"": ""1.0"",
	""source"": ""publisher"",
	""type"": ""com.dapr.event.sent"",
	""topic"": ""__global""
}");
    _httpContextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _httpContextMock
      .Setup(x => x.Request.ContentType)
      .Returns("application/cloudevents+json");

    TopicDelegate sut = TopicDelegate.Create(new[] { Handler2Data }, new[] { typeof(Message2) });

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _handler2Mock.Verify(x => x.HandleAsync(It.Is<Message2>(x => x.Id.ToString() == "27a3e578-d9bc-412b-9a4f-1218b78fd122"), It.IsAny<CancellationToken>()), Times.Once);
    HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
  }

  [Fact]
  public async Task ExecuteAsync_ShouldCallCorrectHandlerWithCorrectMessage_WhenHandlerHasNoResultAndMessageIsRawPayload()
  {
    // Arrange
    using Stream body = CreateJsonBody(
@"{
  ""type"": ""Message1"",
  ""message"": {
    ""id"": ""27a3e578-d9bc-412b-9a4f-1218b78fd122""
  }
}");
    _httpContextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _httpContextMock
      .Setup(x => x.Request.ContentType)
      .Returns("application/json");

    TopicDelegate sut = TopicDelegate.Create(new[] { Handler1Data }, new[] { typeof(Message1) });

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _handler1Mock.Verify(x => x.HandleAsync(It.Is<Message1>(x => x.Id.ToString() == "27a3e578-d9bc-412b-9a4f-1218b78fd122"), It.IsAny<CancellationToken>()), Times.Once);
    HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
  }

  [Fact]
  public async Task ExecuteAsync_ShouldCallCorrectHandlerWithCorrectMessage_WhenHandlerHasResultAndMessageIsRawPayload()
  {
    // Arrange
    using Stream body = CreateJsonBody(
@"{
  ""type"": ""Message2"",
  ""message"": {
    ""id"": ""27a3e578-d9bc-412b-9a4f-1218b78fd122""
  }
}");
    _httpContextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _httpContextMock
      .Setup(x => x.Request.ContentType)
      .Returns("application/json");

    TopicDelegate sut = TopicDelegate.Create(new[] { Handler2Data }, new[] { typeof(Message2) });

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _handler2Mock.Verify(x => x.HandleAsync(It.Is<Message2>(x => x.Id.ToString() == "27a3e578-d9bc-412b-9a4f-1218b78fd122"), It.IsAny<CancellationToken>()), Times.Once);
    HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
  }

  [Fact]
  public async Task ExecuteAsync_ShouldCallNoHandler_WhenNoHandlerIsRegistered()
  {
    // Arrange
    using Stream body = CreateJsonBody(
@"{
  ""type"": ""Message1"",
  ""message"": {
    ""id"": ""27a3e578-d9bc-412b-9a4f-1218b78fd122""
  }
}");
    _httpContextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _httpContextMock
      .Setup(x => x.Request.ContentType)
      .Returns("application/json");

    TopicDelegate sut = TopicDelegate.Create(Enumerable.Empty<TopicDelegateData>(), Type.EmptyTypes);

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _handler1Mock.Verify(x => x.HandleAsync(It.IsAny<Message1>(), It.IsAny<CancellationToken>()), Times.Never);
    _handler2Mock.Verify(x => x.HandleAsync(It.IsAny<Message2>(), It.IsAny<CancellationToken>()), Times.Never);
    _handler3Mock.Verify(x => x.HandleAsync(It.IsAny<Message3>(), It.IsAny<CancellationToken>()), Times.Never);
    HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
  }

  [Fact]
  public async Task ExecuteAsync_ShouldCallCorrectHandlerWithCorrectMessage_WhenMessageIsDerived()
  {
    // Arrange
    using Stream body = CreateJsonBody(
@"{
  ""type"": ""Message3"",
  ""message"": {
    ""id"": ""27a3e578-d9bc-412b-9a4f-1218b78fd122"",
    ""prop"": ""propvalue""
  }
}");
    _httpContextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _httpContextMock
      .Setup(x => x.Request.ContentType)
      .Returns("application/json");

    TopicDelegate sut = TopicDelegate.Create(new[] { Handler1Data, Handler3Data }, new[] { typeof(Message1), typeof(Message3) });

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _handler1Mock.Verify(x => x.HandleAsync(It.IsAny<Message1>(), It.IsAny<CancellationToken>()), Times.Never);
    _handler3Mock.Verify(x => x.HandleAsync(It.Is<Message3>(x => x.Id.ToString() == "27a3e578-d9bc-412b-9a4f-1218b78fd122" && x.Prop == "propvalue"), It.IsAny<CancellationToken>()), Times.Once);
    HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
  }

  [Fact]
  public async Task ExecuteAsync_ShouldCallFallbackHandler_WhenMessageIsDerivedAndNoDerivedHandlerIsRegistered()
  {
    // Arrange
    using Stream body = CreateJsonBody(
@"{
  ""type"": ""Message3"",
  ""message"": {
    ""id"": ""27a3e578-d9bc-412b-9a4f-1218b78fd122"",
    ""prop"": ""propvalue""
  }
}");
    _httpContextMock
      .Setup(x => x.Request.Body)
      .Returns(body);
    _httpContextMock
      .Setup(x => x.Request.ContentType)
      .Returns("application/json");

    TopicDelegate sut = TopicDelegate.Create(new[] { Handler1Data }, new[] { typeof(Message1), typeof(Message3) });

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _handler1Mock.Verify(x => x.HandleAsync(It.Is<Message1>(x => x.Id.ToString() == "27a3e578-d9bc-412b-9a4f-1218b78fd122"), It.IsAny<CancellationToken>()), Times.Once);
    _handler3Mock.Verify(x => x.HandleAsync(It.IsAny<Message3>(), It.IsAny<CancellationToken>()), Times.Never);
    HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
  }

  private static Stream CreateJsonBody(string json)
  {
    return new MemoryStream(Encoding.UTF8.GetBytes(json));
  }
}
