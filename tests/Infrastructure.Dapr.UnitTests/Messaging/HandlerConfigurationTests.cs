using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Common.Internals;
using CodeArchitects.Platform.Infrastructure.Dapr.Messaging.Fakes;
using CodeArchitects.Platform.Infrastructure.Messaging;
using FluentAssertions;
using Moq;
using System;
using System.Reflection;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

public class HandlerConfigurationTests
{
  private readonly Mock<Assembly> _assemblyMock;
  private readonly DaprMessagingOptions _options;

  public HandlerConfigurationTests()
  {
    _assemblyMock = new Mock<Assembly>(MockBehavior.Strict);
    _options = new DaprMessagingOptions();
  }

  [Fact]
  public void Create_ShouldRegisterMessageHandler_WhenTypeHasAttribute()
  {
    // Arrange
    Type handlerType = typeof(Message1Handler1);
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new Type[] { handlerType });

    MessageHandlerIdentity expectedIdentity = new MessageHandlerIdentity(null, MessageBus.DefaultTopic, typeof(Message1));
    ImplementationPair expectedPair = new ImplementationPair(typeof(IMessageHandler<Message1>), handlerType);

    // Act
    MessagingConfiguration configuration = MessagingConfiguration.Create(new Assembly[] { _assemblyMock.Object }, _options);

    // Assert
    configuration.HandlerMap.Keys.Should().HaveCount(1).And.Contain(expectedIdentity);
    configuration.HandlerMap[expectedIdentity].Should().Be(expectedPair);
  }

  [Fact]
  public void Create_ShouldRegisterMessageHandler_WhenMethodHasAttribute()
  {
    // Arrange
    Type handlerType = typeof(Message1Handler2);
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new Type[] { handlerType });

    MessageHandlerIdentity expectedIdentity = new MessageHandlerIdentity(null, MessageBus.DefaultTopic, typeof(Message1));
    ImplementationPair expectedPair = new ImplementationPair(typeof(IMessageHandler<Message1>), handlerType);

    // Act
    MessagingConfiguration configuration = MessagingConfiguration.Create(new Assembly[] { _assemblyMock.Object }, _options);

    // Assert
    configuration.HandlerMap.Keys.Should().HaveCount(1).And.Contain(expectedIdentity);
    configuration.HandlerMap[expectedIdentity].Should().Be(expectedPair);
  }

  [Fact]
  public void Create_ShouldRegisterMessageHandler_WhenTypeHasAttributeWithBusName()
  {
    // Arrange
    Type handlerType = typeof(Message1Handler3);
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new Type[] { handlerType });

    MessageHandlerIdentity expectedIdentity = new MessageHandlerIdentity(Message1Handler3.BusName, MessageBus.DefaultTopic, typeof(Message1));
    ImplementationPair expectedPair = new ImplementationPair(typeof(IMessageHandler<Message1>), handlerType);

    // Act
    MessagingConfiguration configuration = MessagingConfiguration.Create(new Assembly[] { _assemblyMock.Object }, _options);

    // Assert
    configuration.HandlerMap.Keys.Should().HaveCount(1).And.Contain(expectedIdentity);
    configuration.HandlerMap[expectedIdentity].Should().Be(expectedPair);
  }

  [Fact]
  public void Create_ShouldRegisterMessageHandler_WhenMethodHasAttributeWithBusName()
  {
    // Arrange
    Type handlerType = typeof(Message1Handler4);
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new Type[] { handlerType });

    MessageHandlerIdentity expectedIdentity = new MessageHandlerIdentity(Message1Handler4.BusNameFromMethod, MessageBus.DefaultTopic, typeof(Message1));
    ImplementationPair expectedPair = new ImplementationPair(typeof(IMessageHandler<Message1>), handlerType);

    // Act
    MessagingConfiguration configuration = MessagingConfiguration.Create(new Assembly[] { _assemblyMock.Object }, _options);

    // Assert
    configuration.HandlerMap.Keys.Should().HaveCount(1).And.Contain(expectedIdentity);
    configuration.HandlerMap[expectedIdentity].Should().Be(expectedPair);
  }

  [Fact]
  public void Create_ShouldRegisterMessageHandler_WhenTypeHasAttributeWithTopic()
  {
    // Arrange
    Type handlerType = typeof(Message1Handler5);
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new Type[] { handlerType });

    MessageHandlerIdentity expectedIdentity = new MessageHandlerIdentity(null, Message1Handler5.Topic, typeof(Message1));
    ImplementationPair expectedPair = new ImplementationPair(typeof(IMessageHandler<Message1>), handlerType);

    // Act
    MessagingConfiguration configuration = MessagingConfiguration.Create(new Assembly[] { _assemblyMock.Object }, _options);

    // Assert
    configuration.HandlerMap.Keys.Should().HaveCount(1).And.Contain(expectedIdentity);
    configuration.HandlerMap[expectedIdentity].Should().Be(expectedPair);
  }

  [Fact]
  public void Create_ShouldRegisterMessageHandler_WhenMethodHasAttributeWithTopic()
  {
    // Arrange
    Type handlerType = typeof(Message1Handler6);
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new Type[] { handlerType });

    MessageHandlerIdentity expectedIdentity = new MessageHandlerIdentity(null, Message1Handler6.TopicFromMethod, typeof(Message1));
    ImplementationPair expectedPair = new ImplementationPair(typeof(IMessageHandler<Message1>), handlerType);

    // Act
    MessagingConfiguration configuration = MessagingConfiguration.Create(new Assembly[] { _assemblyMock.Object }, _options);

    // Assert
    configuration.HandlerMap.Keys.Should().HaveCount(1).And.Contain(expectedIdentity);
    configuration.HandlerMap[expectedIdentity].Should().Be(expectedPair);
  }

  [Fact]
  public void Create_ShouldRegisterMessageHandler_WhenItHasReturnValue()
  {
    // Arrange
    Type handlerType = typeof(Message1Handler7);
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new Type[] { handlerType });

    MessageHandlerIdentity expectedIdentity = new MessageHandlerIdentity(null, MessageBus.DefaultTopic, typeof(Message1));
    ImplementationPair expectedPair = new ImplementationPair(typeof(IMessageHandler<Message1, string>), handlerType);

    // Act
    MessagingConfiguration configuration = MessagingConfiguration.Create(new Assembly[] { _assemblyMock.Object }, _options);

    // Assert
    configuration.HandlerMap.Keys.Should().HaveCount(1).And.Contain(expectedIdentity);
    configuration.HandlerMap[expectedIdentity].Should().Be(expectedPair);
  }

  [Fact]
  public void Create_ShouldRegisterMessageHandler_WhenItHandlesMultipleMessages()
  {
    // Arrange
    Type handlerType = typeof(Message1And2Handler);
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new Type[] { handlerType });

    MessageHandlerIdentity expectedIdentity1 = new MessageHandlerIdentity(null, MessageBus.DefaultTopic, typeof(Message1));
    MessageHandlerIdentity expectedIdentity2 = new MessageHandlerIdentity(null, MessageBus.DefaultTopic, typeof(Message2));
    ImplementationPair expectedPair1 = new ImplementationPair(typeof(IMessageHandler<Message1>), handlerType);
    ImplementationPair expectedPair2 = new ImplementationPair(typeof(IMessageHandler<Message2, string>), handlerType);

    // Act
    MessagingConfiguration configuration = MessagingConfiguration.Create(new Assembly[] { _assemblyMock.Object }, _options);

    // Assert
    configuration.HandlerMap.Keys.Should().HaveCount(2)
      .And.Contain(expectedIdentity1)
      .And.Contain(expectedIdentity2);
    configuration.HandlerMap[expectedIdentity1].Should().Be(expectedPair1);
    configuration.HandlerMap[expectedIdentity2].Should().Be(expectedPair2);
  }

  [Fact]
  public void Create_ShouldNotRegisterMessageHandler_WhenItDoesNotHaveAttributes()
  {
    // Arrange
    Type handlerType = typeof(ThisHandlerShouldntBeRegistered);
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new Type[] { handlerType });

    // Act
    MessagingConfiguration configuration = MessagingConfiguration.Create(new Assembly[] { _assemblyMock.Object }, _options);

    // Assert
    configuration.HandlerMap.Keys.Should().BeEmpty();
  }

  [Fact]
  public void Create_ShouldRegisterMessagesOfHandlersAndMessagesWithAttribute()
  {
    // Arrange
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new Type[] { typeof(Message1And2Handler), typeof(Message3) });

    // Act
    MessagingConfiguration configuration = MessagingConfiguration.Create(new Assembly[] { _assemblyMock.Object }, _options);

    // Assert
    configuration.MessageTypes.Should().HaveCount(3)
      .And.Contain(typeof(Message1))
      .And.Contain(typeof(Message2))
      .And.Contain(typeof(Message3));
  }

  [Fact]
  public void Create_ShouldThrowServiceRegistrationException_WhenMultipleHandlersHaveSameIdentity()
  {
    // Arrange
    Type[] handlerTypes = new Type[] { typeof(Message1Handler1), typeof(Message1Handler7) };
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(handlerTypes);

    // Act
    Func<MessagingConfiguration> act = () => MessagingConfiguration.Create(new Assembly[] { _assemblyMock.Object }, _options);

    // Assert
    act.Should().Throw<ServiceRegistrationException>().Which.ImplementationTypes.Should().BeEquivalentTo(handlerTypes);
  }
}
