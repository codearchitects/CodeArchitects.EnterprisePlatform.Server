using FluentAssertions;
using Xunit;

using static CodeArchitects.Platform.Messaging.Descriptors.Reflection.HandlerDescriptorFixture;

namespace CodeArchitects.Platform.Messaging.Descriptors.Reflection;

public class HandlerDescriptorTests
{
  [Theory]
  [StandardMessageHandlerData]
  internal void Create_ShouldCreateCorrectHandlerDescriptor(IEnumerable<Type> concreteTypes, string? defaultBus, string? defaultTopic, IMessagingDescriptor expected)
  {
    // Arrange

    // Act
    MessagingDescriptor actual = MessagingDescriptor.Create(concreteTypes, defaultBus, defaultTopic);

    // Assert
    actual.Should().BeEquivalentTo(expected, x => x.Using<IMessagingDescriptor>(HandlerDescriptorEqualityComparer.Instance));
  }
}
