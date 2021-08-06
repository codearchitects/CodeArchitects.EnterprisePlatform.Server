using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using CodeArchitects.Platform.Infrastructure.Dapr.Tests.Fixtures;
using FluentAssertions;
using Moq;
using System;
using System.Reflection;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Tests.Messaging
{
  public class HandlerConfigurationTests
  {
    [Fact]
    public void Constructor_ShouldCreateHandlerMap_WhenAssemblyIsValid()
    {
      // Arrange
      HandlerIdentity[] expectedIdentities = HandlerAssembly.ExpectedIdentities;

      // Act
      HandlerConfiguration configuration = new HandlerConfiguration(new Assembly[] { HandlerAssembly.Valid.Instance });

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
      HandlerIdentity[] expectedIdentities = new HandlerIdentity[]
      {
        new HandlerIdentity(null, null, typeof(FakeMessage1)), // Handlers: FakeMessage1Handler1
        new HandlerIdentity(null, null, typeof(FakeMessage2)), // Handlers: FakeMessage2Handler
      };
      Type expectedHandlerForIdentity0 = typeof(FakeMessage1Handler1);
      Type expectedHandlerForIdentity1 = typeof(FakeMessage2Handler);

      // Act
      HandlerConfiguration configuration = new HandlerConfiguration(new Assembly[] { assembly1Mock.Object, assembly2Mock.Object });

      // Assert
      configuration.HandlerMap.Keys.Should().BeEquivalentTo(expectedIdentities);
      configuration.HandlerMap[expectedIdentities[0]].Should().Be(expectedHandlerForIdentity0);
      configuration.HandlerMap[expectedIdentities[1]].Should().Be(expectedHandlerForIdentity1);
    }

    [Fact]
    public void Constructor_ShouldThrowServiceRegistrationException_WhenAssemblyIsInvalid()
    {
      // Arrange
      HandlerIdentity[] expectedIdentities = HandlerAssembly.ExpectedIdentities;

      // Act
      Func<HandlerConfiguration> act = () => new HandlerConfiguration(new Assembly[] { HandlerAssembly.Invalid.Instance });

      // Assert
      act.Should().Throw<ServiceRegistrationException>().Which.ImplementationTypes.Should().BeEquivalentTo(HandlerAssembly.Invalid.InvalidHandlerTypes);
    }
  }
}
