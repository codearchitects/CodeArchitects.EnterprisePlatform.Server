using CodeArchitects.Platform.Messaging.Fixtures;
using FluentAssertions;
using Xunit;

using static CodeArchitects.Platform.Messaging.Descriptors.Concrete.HandlerDescriptorFixture;

namespace CodeArchitects.Platform.Messaging.Descriptors.Concrete;

public class HandlerDescriptorTests
{
  [Theory, StandardHandlerData]
  internal void Create_ShouldCreateCorrectActorDescriptor_WhenActorIsNonVirtual(IHandlerDescriptor expected)
  {
    // Arrange

    // Act
    HandlerDescriptor actual = HandlerDescriptor.Create(null, null, typeof(StandardMessageHandler));

    // Assert
    actual.Should().BeEquivalentTo(expected, x => x.Using<IHandlerDescriptor>(HandlerDescriptorEqualityComparer.Instance));
  }
}
