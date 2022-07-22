using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using CodeArchitects.Platform.Messaging.AspNetCore.Fixtures;
using CodeArchitects.Platform.Messaging.AspNetCore.Handlers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace CodeArchitects.Platform.Messaging.AspNetCore.HandlerDelegates;

public class HandlerDelegate3Tests
{
  private readonly HandlerDelegate<Message2Handler, Message2, object> _sut;
  private readonly Mock<OutputAction> _outputActionMock;

  public HandlerDelegate3Tests()
  {
    _outputActionMock = new(MockBehavior.Loose);
    _sut = new HandlerDelegate<Message2Handler, Message2, object>(new[] { _outputActionMock.Object });
  }

  [Fact]
  public async Task HandleAsync_ShouldCallHandlerWithMessageAndOutputActionsWithResult()
  {
    // Arrange
    Message2 message = new Message2 { Data = "myData" };
    JObject messageJson = JObject.FromObject(message);
    object result = new();

    Mock<Message2Handler> message2HandlerMock = new(MockBehavior.Strict);
    message2HandlerMock
      .Setup(x => x.HandleAsync(It.IsAny<Message2>(), It.IsAny<CancellationToken>()))
      .Returns(Task.FromResult(result));

    Mock<HttpContext> httpContextMock = new(MockBehavior.Strict);
    httpContextMock
      .Setup(x => x.RequestServices.GetService(typeof(Message2Handler)))
      .Returns(message2HandlerMock.Object);
    httpContextMock
      .Setup(x => x.RequestAborted)
      .Returns(CancellationToken.None);

    // Act
    await _sut.HandleAsync(httpContextMock.Object, messageJson);

    // Assert
    message2HandlerMock.Verify(x => x.HandleAsync(It.Is<Message2>(m => m.Data == message.Data), CancellationToken.None));
    _outputActionMock.Verify(x => x.ExecuteAsync(It.Is<Message2>(m => m.Data == message.Data), result, CancellationToken.None));
  }

  [Fact]
  public async Task HandleAsync_ShouldThrowInvalidMessageTypeException_WhenMessageTypeIsInvalid()
  {
    // Arrange
    JObject messageJson = JObject.Parse("{ \"Data\": {} }");

    // Act
    Func<Task> act = () => _sut.HandleAsync(Mock.Of<HttpContext>(), messageJson);

    // Assert
    (await act.Should().ThrowAsync<InvalidMessageTypeException>()).Which.MessageType.Should().Be<Message2>();
  }
}
