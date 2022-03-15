using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Fakes;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

public class TopicDelegateTests
{
  private readonly Mock<HandlerDelegate> _delegate1Mock;
  private readonly Mock<HandlerDelegate> _delegate2Mock;
  private readonly Mock<HandlerDelegate> _delegate3Mock;
  private readonly Mock<IServiceProvider> _serviceProviderMock;
  private readonly Mock<HttpContext> _httpContextMock;

  public TopicDelegateTests()
  {
    _delegate1Mock = new Mock<HandlerDelegate>(MockBehavior.Strict);
    _delegate1Mock
      .Setup(x => x(It.IsAny<HttpContext>(), It.IsAny<JObject>()))
      .Returns(Task.CompletedTask);

    _delegate2Mock = new Mock<HandlerDelegate>(MockBehavior.Strict);
    _delegate2Mock
      .Setup(x => x(It.IsAny<HttpContext>(), It.IsAny<JObject>()))
      .Returns(Task.CompletedTask);

    _delegate3Mock = new Mock<HandlerDelegate>(MockBehavior.Strict);
    _delegate3Mock
      .Setup(x => x(It.IsAny<HttpContext>(), It.IsAny<JObject>()))
      .Returns(Task.CompletedTask);

    Mock<ILoggerFactory> loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
    Mock<ILogger> loggerMock = new Mock<ILogger>(MockBehavior.Loose);
    loggerFactoryMock
      .Setup(x => x.CreateLogger("CAEP-Messaging"))
      .Returns(loggerMock.Object);
    _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
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

  private HandlerDelegate Delegate1 => _delegate1Mock.Object;
  private HandlerDelegate Delegate2 => _delegate2Mock.Object;
  private HandlerDelegate Delegate3 => _delegate3Mock.Object;
  private IServiceProvider ServiceProvider => _serviceProviderMock.Object;
  private HttpContext HttpContext => _httpContextMock.Object;

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

    Dictionary<string, HandlerDelegate> delegates = new Dictionary<string, HandlerDelegate>
    {
      ["Message1"] = Delegate1
    };
    Dictionary<string, Type> messageTypes = new Dictionary<string, Type>
    {
      ["Message1"] = typeof(Message1)
    };

    TopicDelegate sut = new TopicDelegate(delegates, messageTypes);

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _delegate1Mock.Verify(x => x(_httpContextMock.Object, It.Is<JObject>(x => x.Value<string>("id") == "27a3e578-d9bc-412b-9a4f-1218b78fd122")), Times.Once);
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

    Dictionary<string, HandlerDelegate> delegates = new Dictionary<string, HandlerDelegate>
    {
      ["Message2"] = Delegate2
    };
    Dictionary<string, Type> messageTypes = new Dictionary<string, Type>
    {
      ["Message2"] = typeof(Message2)
    };

    TopicDelegate sut = new TopicDelegate(delegates, messageTypes);

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _delegate2Mock.Verify(x => x(_httpContextMock.Object, It.Is<JObject>(x => x.Value<string>("id") == "27a3e578-d9bc-412b-9a4f-1218b78fd122")), Times.Once);
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

    Dictionary<string, HandlerDelegate> delegates = new Dictionary<string, HandlerDelegate>
    {
      ["Message1"] = Delegate1
    };
    Dictionary<string, Type> messageTypes = new Dictionary<string, Type>
    {
      ["Message1"] = typeof(Message1)
    };

    TopicDelegate sut = new TopicDelegate(delegates, messageTypes);

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _delegate1Mock.Verify(x => x(_httpContextMock.Object, It.Is<JObject>(x => x.Value<string>("id") == "27a3e578-d9bc-412b-9a4f-1218b78fd122")), Times.Once);
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

    Dictionary<string, HandlerDelegate> delegates = new Dictionary<string, HandlerDelegate>
    {
      ["Message2"] = Delegate2
    };
    Dictionary<string, Type> messageTypes = new Dictionary<string, Type>
    {
      ["Message2"] = typeof(Message2)
    };

    TopicDelegate sut = new TopicDelegate(delegates, messageTypes);

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _delegate2Mock.Verify(x => x(_httpContextMock.Object, It.Is<JObject>(x => x.Value<string>("id") == "27a3e578-d9bc-412b-9a4f-1218b78fd122")), Times.Once);
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

    Dictionary<string, HandlerDelegate> delegates = new Dictionary<string, HandlerDelegate>();
    Dictionary<string, Type> messageTypes = new Dictionary<string, Type>();

    TopicDelegate sut = new TopicDelegate(delegates, messageTypes);

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _delegate1Mock.Verify(x => x(_httpContextMock.Object, It.IsAny<JObject>()), Times.Never);
    _delegate2Mock.Verify(x => x(_httpContextMock.Object, It.IsAny<JObject>()), Times.Never);
    _delegate3Mock.Verify(x => x(_httpContextMock.Object, It.IsAny<JObject>()), Times.Never);
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

    Dictionary<string, HandlerDelegate> delegates = new Dictionary<string, HandlerDelegate>
    {
      ["Message1"] = Delegate1,
      ["Message3"] = Delegate3
    };
    Dictionary<string, Type> messageTypes = new Dictionary<string, Type>
    {
      ["Message1"] = typeof(Message1),
      ["Message3"] = typeof(Message3),
    };

    TopicDelegate sut = new TopicDelegate(delegates, messageTypes);

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _delegate1Mock.Verify(x => x(_httpContextMock.Object, It.IsAny<JObject>()), Times.Never);
    _delegate3Mock.Verify(x => x(_httpContextMock.Object, It.Is<JObject>(x => x.Value<string>("id") == "27a3e578-d9bc-412b-9a4f-1218b78fd122" && x.Value<string>("prop") == "propvalue")), Times.Once);
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

    Dictionary<string, HandlerDelegate> delegates = new Dictionary<string, HandlerDelegate>
    {
      ["Message1"] = Delegate1
    };
    Dictionary<string, Type> messageTypes = new Dictionary<string, Type>
    {
      ["Message1"] = typeof(Message1),
      ["Message3"] = typeof(Message3),
    };

    TopicDelegate sut = new TopicDelegate(delegates, messageTypes);

    // Act
    await sut.ExecuteAsync(_httpContextMock.Object);

    // Assert
    _delegate1Mock.Verify(x => x(_httpContextMock.Object, It.Is<JObject>(x => x.Value<string>("id") == "27a3e578-d9bc-412b-9a4f-1218b78fd122" && x.Value<string>("prop") == "propvalue")), Times.Once);
    _delegate3Mock.Verify(x => x(_httpContextMock.Object, It.IsAny<JObject>()), Times.Never);
    HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
  }

  private static Stream CreateJsonBody(string json)
  {
    return new MemoryStream(Encoding.UTF8.GetBytes(json));
  }
}
