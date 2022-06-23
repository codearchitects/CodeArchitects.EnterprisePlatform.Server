using CodeArchitects.Platform.Messaging.Fixtures;
using FluentAssertions;
using Xunit;

using static CodeArchitects.Platform.Messaging.Descriptors.Reflection.HandlerDescriptorFixture;

namespace CodeArchitects.Platform.Messaging.Descriptors.Reflection;

public class HandlerDescriptorTests
{
  [Theory, StandardMessageHandlerData]
  internal void Create_ShouldCreateCorrectHandlerDescriptor_WhenBusAndTopicAreDefined(IHandlerDescriptor expected)
  {
    // Arrange

    // Act
    HandlerDescriptor actual = HandlerDescriptor.Create(null, null, typeof(StandardMessageHandler));

    // Assert
    actual.Should().BeEquivalentTo(expected, x => x.Using<IHandlerDescriptor>(HandlerDescriptorEqualityComparer.Instance));
  }

  [Theory, NoBusAndTopicMessageHandlerData]
  internal void Create_ShouldCreateCorrectHandlerDescriptor_WhenBusAndTopicAreNotDefined(string defaultBus, string defaultTopic, IHandlerDescriptor expected)
  {
    // Arrange

    // Act
    HandlerDescriptor actual = HandlerDescriptor.Create(defaultBus, defaultTopic, typeof(NoBusAndTopicMessageHandler));

    // Assert
    actual.Should().BeEquivalentTo(expected, x => x.Using<IHandlerDescriptor>(HandlerDescriptorEqualityComparer.Instance));
  }
}
