using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using CodeArchitects.Platform.Messaging.AspNetCore.Fixtures;
using CodeArchitects.Platform.Messaging.AspNetCore.Handlers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace CodeArchitects.Platform.Messaging.AspNetCore.HandlerDelegates;

public class HandlerDelegate2Tests
{
  private readonly HandlerDelegate<Message1Handler, Message1> _sut;
  private readonly Mock<OutputAction> _outputActionMock;

  public HandlerDelegate2Tests()
  {
    _outputActionMock = new(MockBehavior.Loose);
    _sut = new HandlerDelegate<Message1Handler, Message1>(new[] { _outputActionMock.Object });
  }

  [Fact]
  public async Task HandleAsync_ShouldCallHandlerWithMessageAndOutputActionsWithNullResult()
  {
    // Arrange
    Message1 message = new()
    { Data = "myData" };
    JObject messageJson = JObject.FromObject(message);

    Mock<Message1Handler> message1HandlerMock = new(MockBehavior.Strict);
    message1HandlerMock
      .Setup(x => x.HandleAsync(It.IsAny<Message1>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    Mock<HttpContext> httpContextMock = new(MockBehavior.Strict);
    httpContextMock
      .Setup(x => x.RequestServices.GetService(typeof(Message1Handler)))
      .Returns(message1HandlerMock.Object);
    httpContextMock
      .Setup(x => x.RequestAborted)
      .Returns(CancellationToken.None);

    // Act
    await _sut.HandleAsync(httpContextMock.Object, messageJson);

    // Assert
    message1HandlerMock.Verify(x => x.HandleAsync(It.Is<Message1>(m => m.Data == message.Data), CancellationToken.None));
    _outputActionMock.Verify(x => x.ExecuteAsync(It.Is<Message1>(m => m.Data == message.Data), default(object), CancellationToken.None));
  }

  [Fact]
  public async Task HandleAsync_ShouldThrowInvalidMessageTypeException_WhenMessageTypeIsInvalid()
  {
    // Arrange
    JObject messageJson = JObject.Parse("{ \"Data\": {} }");

    // Act
    Func<Task> act = () => _sut.HandleAsync(Mock.Of<HttpContext>(), messageJson);

    // Assert
    (await act.Should().ThrowAsync<InvalidMessageTypeException>()).Which.MessageType.Should().Be<Message1>();
  }
}
