using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Infrastructure.Dapr.Tests;
using FluentAssertions;
using Moq;
using System;
using System.Reflection;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging
{
  public class HandlerConfigurationTests
  {
    [Fact]
    public void Constructor_ShouldCreateHandlerMap_WhenAssemblyIsValid()
    {
      // Arrange
      MessageHandlerIdentity[] expectedIdentities = HandlerAssembly.ExpectedIdentities;

      // Act
      MessageHandlerConfiguration configuration = new MessageHandlerConfiguration(new Assembly[] { HandlerAssembly.Valid.Instance });

      // Assert
      configuration.HandlerMap.Keys.Should().BeEquivalentTo(expectedIdentities);
      configuration.HandlerMap[expectedIdentities[0]].Should().Be(HandlerAssembly.Valid.HandlerForIdentity0);
      configuration.HandlerMap[expectedIdentities[1]].Should().Be(HandlerAssembly.Valid.HandlerForIdentity1);
      configuration.HandlerMap[expectedIdentities[2]].Should().Be(HandlerAssembly.Valid.HandlerForIdentity2);
      configuration.HandlerMap[expectedIdentities[3]].Should().Be(HandlerAssembly.Valid.HandlerForIdentity3);
    }

    [Fact]
    public void Constructor_ShouldCreateHandlerMapFromAllAssemblies_WhenAssembliesAreValid()
    {
      // Arrange
      Mock<Assembly> assembly1Mock = new Mock<Assembly>(behavior: MockBehavior.Strict);
      assembly1Mock
        .Setup(x => x.GetTypes())
        .Returns(new Type[]
        {
          typeof(FakeMessage1Handler1) // Identities: (null, null, typeof(FakeMessage1))
        });
      Mock<Assembly> assembly2Mock = new Mock<Assembly>(behavior: MockBehavior.Strict);
      assembly2Mock
        .Setup(x => x.GetTypes())
        .Returns(new Type[]
        {
          typeof(FakeMessage2Handler) // Identities: (null, null, typeof(FakeMessage2))
        });
      MessageHandlerIdentity[] expectedIdentities = new MessageHandlerIdentity[]
      {
        new MessageHandlerIdentity(null, null, typeof(FakeMessage1)), // Handlers: FakeMessage1Handler1
        new MessageHandlerIdentity(null, null, typeof(FakeMessage2)), // Handlers: FakeMessage2Handler
      };
      Type expectedHandlerForIdentity0 = typeof(FakeMessage1Handler1);
      Type expectedHandlerForIdentity1 = typeof(FakeMessage2Handler);

      // Act
      MessageHandlerConfiguration configuration = new MessageHandlerConfiguration(new Assembly[] { assembly1Mock.Object, assembly2Mock.Object });

      // Assert
      configuration.HandlerMap.Keys.Should().BeEquivalentTo(expectedIdentities);
      configuration.HandlerMap[expectedIdentities[0]].Should().Be(expectedHandlerForIdentity0);
      configuration.HandlerMap[expectedIdentities[1]].Should().Be(expectedHandlerForIdentity1);
    }

    [Fact]
    public void Constructor_ShouldThrowServiceRegistrationException_WhenAssemblyIsInvalid()
    {
      // Arrange
      MessageHandlerIdentity[] expectedIdentities = HandlerAssembly.ExpectedIdentities;

      // Act
      Func<MessageHandlerConfiguration> act = () => new MessageHandlerConfiguration(new Assembly[] { HandlerAssembly.Invalid.Instance });

      // Assert
      act.Should().Throw<ServiceRegistrationException>().Which.ImplementationTypes.Should().BeEquivalentTo(HandlerAssembly.Invalid.InvalidHandlerTypes);
    }
  }
}
