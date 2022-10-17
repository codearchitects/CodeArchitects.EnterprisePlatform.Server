using CodeArchitects.Platform.Messaging.AspNetCore.Fixtures;
using CodeArchitects.Platform.Messaging.Bindings;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

public class OutputAction1Tests
{
  private readonly Mock<IServiceProvider> _servicesMock;
  private readonly OutputMetadata _metadata;
  private readonly OutputAction<OutputMetadata> _sut;

  public OutputAction1Tests()
  {
    _servicesMock = new(MockBehavior.Strict);
    _metadata = new OutputMetadata();
    _sut = new OutputAction<OutputMetadata>(_metadata, _servicesMock.Object);
  }

  [Fact]
  public void ExecuteAsync_ShouldReturnCompletedTask_WhenBindingAndLoggerFactoryAreNotRegistered()
  {
    // Arrange
    _servicesMock
      .Setup(x => x.GetService(typeof(IOutputBinding<OutputMetadata>)))
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
      .Setup(x => x.GetService(typeof(IOutputBinding<OutputMetadata>)))
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

    Mock<IOutputBinding<OutputMetadata>> bindingMock = new(MockBehavior.Strict);
    bindingMock
      .Setup(x => x.ExecuteAsync(It.IsAny<OutputBindingContext<OutputMetadata, object, object>>(), It.IsAny<CancellationToken>()))
      .Returns(expected);

    _servicesMock
      .Setup(x => x.GetService(typeof(IOutputBinding<OutputMetadata>)))
      .Returns(bindingMock.Object);

    // Act
    Task task = _sut.ExecuteAsync(message, result, cancellationToken);

    // Assert
    task.Should().BeSameAs(expected);
    bindingMock.Verify(x => x.ExecuteAsync(It.Is<OutputBindingContext<OutputMetadata, object, object>>(ctx => ctx.Message == message && ctx.Result == result && ctx.Metadata == _metadata), cancellationToken));
  }
}
