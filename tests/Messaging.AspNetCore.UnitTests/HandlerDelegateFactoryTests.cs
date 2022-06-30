using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using CodeArchitects.Platform.Messaging.AspNetCore.Fixtures;
using CodeArchitects.Platform.Messaging.Descriptors;
using FluentAssertions;
using Moq;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

public class HandlerDelegateFactoryTests
{
  private readonly HandlerDelegateFactory _sut;

  public HandlerDelegateFactoryTests()
  {
    IServiceProvider services = Mock.Of<IServiceProvider>();
    IOutputActionFactory outputActionFactory = Mock.Of<IOutputActionFactory>(factory => factory.CreateOutputAction(It.IsAny<Type>(), It.IsAny<object>(), services) == Mock.Of<OutputAction>());
    _sut = new(services, outputActionFactory);
  }

  [Fact]
  public void CreateHandlerDelegate_ShouldCreateHandlerDelegateNoResult_WhenHandlerResultTypeIsVoid()
  {
    // Arrange
    Mock<IHandlerDescriptor> descriptorMock = new(MockBehavior.Strict);
    descriptorMock
      .Setup(x => x.OutputBindingDescriptors)
      .Returns(new[] { Mock.Of<IOutputBindingDescriptor>(descr => descr.MetadataType == typeof(object) && descr.MetadataObject == new object()) });
    descriptorMock
      .Setup(x => x.ResultType)
      .Returns(typeof(void));
    descriptorMock
      .Setup(x => x.MessageType)
      .Returns(typeof(Message1));
    descriptorMock
      .Setup(x => x.ConcreteType)
      .Returns(typeof(Message1Handler));

    // Act
    HandlerDelegate @delegate = _sut.CreateHandlerDelegate(descriptorMock.Object);

    // Assert
    @delegate.Should().BeOfType<HandlerDelegate<Message1, Message1Handler>>();
  }

  [Fact]
  public void CreateHandlerDelegate_ShouldCreateHandlerDelegateWithResult_WhenHandlerResultTypeIsNotVoid()
  {
    // Arrange
    Mock<IHandlerDescriptor> descriptorMock = new(MockBehavior.Strict);
    descriptorMock
      .Setup(x => x.OutputBindingDescriptors)
      .Returns(new[] { Mock.Of<IOutputBindingDescriptor>(descr => descr.MetadataType == typeof(object) && descr.MetadataObject == new object()) });
    descriptorMock
      .Setup(x => x.ResultType)
      .Returns(typeof(object));
    descriptorMock
      .Setup(x => x.MessageType)
      .Returns(typeof(Message2));
    descriptorMock
      .Setup(x => x.ConcreteType)
      .Returns(typeof(Message2Handler));

    // Act
    HandlerDelegate @delegate = _sut.CreateHandlerDelegate(descriptorMock.Object);

    // Assert
    @delegate.Should().BeOfType<HandlerDelegate<Message2, object, Message2Handler>>();
  }
}
