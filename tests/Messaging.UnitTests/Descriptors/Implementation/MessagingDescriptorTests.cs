using FluentAssertions;
using static CodeArchitects.Platform.Messaging.Descriptors.Implementation.MessagingDescriptorFixture;

namespace CodeArchitects.Platform.Messaging.Descriptors.Implementation;

public class MessagingDescriptorTests
{
  [Theory]
  [StandardMessageHandlerData]
  internal void Create_ShouldCreateCorrectMessagingDescriptor(IEnumerable<Type> concreteTypes, IEnumerable<Type> messageTypes, string? defaultBus, string? defaultTopic, IMessagingDescriptor expected)
  {
    // Arrange
    List<HandlerDiagnostics> diagnostics = new();

    // Act
    IMessagingDescriptor actual = MessagingDescriptor.Create(concreteTypes, messageTypes, defaultBus, defaultTopic, diagnostics);

    // Assert
    actual.Should().BeEquivalentTo(expected, x => x.Using<IMessagingDescriptor>(HandlerDescriptorEqualityComparer.Instance));
  }

  [Theory]
  [MergeData]
  internal void Merge_ShouldMergeMessagingDescriptorsCorrectly(IMessagingDescriptor first, IMessagingDescriptor second, IMessagingDescriptor expected)
  {
    // Arrange
    List<HandlerDiagnostics> diagnostics = new();

    // Act
    IMessagingDescriptor actual = MessagingDescriptor.Merge(first, second, diagnostics);

    // Assert
    actual.Should().BeEquivalentTo(expected, x => x.Using<IMessagingDescriptor>(HandlerDescriptorEqualityComparer.Instance));
  }
}
