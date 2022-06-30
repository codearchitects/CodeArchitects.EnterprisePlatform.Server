using CodeArchitects.Platform.Messaging.Bindings;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

public class OutputAction1Tests
{
  private readonly Mock<IServiceProvider> _servicesMock;
  private readonly object _metadata;
  private readonly OutputAction<object> _sut;

  public OutputAction1Tests()
  {
    _servicesMock = new(MockBehavior.Strict);
    _metadata = new object();
    _sut = new OutputAction<object>(_servicesMock.Object, _metadata);
  }

  [Fact]
  public void ExecuteAsync_ShouldReturnCompletedTask_WhenBindingAndLoggerFactoryAreNotRegistered()
  {
    // Arrange
    _servicesMock
      .Setup(x => x.GetService(typeof(IOutputBinding<object>)))
      .Returns(null);
    _servicesMock
      .Setup(x => x.GetService(typeof(ILoggerFactory)))
      .Returns(null);

    // Act
    Task task = _sut.ExecuteAsync(new object(), new object(), CancellationToken.None);

    // Assert
    task.Should().BeSameAs(Task.CompletedTask);
  }

  [Fact]
  public void ExecuteAsync_ShouldReturnCompletedTaskAndLogWarning_WhenBindingIsNotRegistered()
  {
    // Arrange
    Mock<ILogger> loggerMock = new(MockBehavior.Loose);

    _servicesMock
      .Setup(x => x.GetService(typeof(IOutputBinding<object>)))
      .Returns(null);
    _servicesMock
      .Setup(x => x.GetService(typeof(ILoggerFactory)))
      .Returns(Mock.Of<ILoggerFactory>(factory => factory.CreateLogger(It.IsAny<string>()) == loggerMock.Object));

    // Act
    Task task = _sut.ExecuteAsync(new object(), new object(), CancellationToken.None);

    // Assert
    task.Should().BeSameAs(Task.CompletedTask);
    loggerMock.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
  }

  [Fact]
  public void ExecuteAsync_ShouldCallBinding_WhenBindingIsRegistered()
  {
    // Arrange
    Task expected = new Task(() => { });
    object message = new object();
    object? result = new object();
    CancellationToken cancellationToken = new CancellationTokenSource().Token;

    Mock<IOutputBinding<object>> bindingMock = new(MockBehavior.Strict);
    bindingMock
      .Setup(x => x.ExecuteAsync(It.IsAny<OutputBindingContext<object, object, object>>(), It.IsAny<CancellationToken>()))
      .Returns(expected);

    _servicesMock
      .Setup(x => x.GetService(typeof(IOutputBinding<object>)))
      .Returns(bindingMock.Object);

    // Act
    Task task = _sut.ExecuteAsync(message, result, cancellationToken);

    // Assert
    task.Should().BeSameAs(expected);
    bindingMock.Verify(x => x.ExecuteAsync(It.Is<OutputBindingContext<object, object, object>>(ctx => ctx.Message == message && ctx.Result == result && ctx.Metadata == _metadata), cancellationToken));
  }
}
