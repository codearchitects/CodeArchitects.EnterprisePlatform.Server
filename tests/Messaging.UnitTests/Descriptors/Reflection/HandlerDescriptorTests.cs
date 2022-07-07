using FluentAssertions;
using static CodeArchitects.Platform.Messaging.Descriptors.Implementation.HandlerDescriptorFixture;

namespace CodeArchitects.Platform.Messaging.Descriptors.Implementation;

public class HandlerDescriptorTests
{
  [Theory]
  [StandardMessageHandlerData]
  internal void Create_ShouldCreateCorrectHandlerDescriptor(IEnumerable<Type> concreteTypes, IEnumerable<Type> messageTypes, string? defaultBus, string? defaultTopic, IMessagingDescriptor expected)
  {
    // Arrange

    // Act
    MessagingDescriptor actual = MessagingDescriptor.Create(concreteTypes, messageTypes, defaultBus, defaultTopic);

    // Assert
    actual.Should().BeEquivalentTo(expected, x => x.Using<IMessagingDescriptor>(HandlerDescriptorEqualityComparer.Instance));
  }
}
