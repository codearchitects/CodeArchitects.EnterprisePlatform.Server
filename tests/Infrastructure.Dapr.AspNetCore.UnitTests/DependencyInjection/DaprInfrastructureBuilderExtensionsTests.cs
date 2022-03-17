using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging.Fakes;
using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using CodeArchitects.Platform.Infrastructure.Messaging;
using Dapr.Client;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.DependencyInjection;

public class DaprInfrastructureBuilderExtensionsTests
{
  private readonly Mock<DaprClient> _daprClientMock;
  private readonly Mock<Assembly> _assemblyMock;
  private readonly ServiceCollection _services;
  private readonly DaprConfiguration _configuration;

  public DaprInfrastructureBuilderExtensionsTests()
  {
    _daprClientMock = new Mock<DaprClient>(MockBehavior.Strict);
    _assemblyMock = new Mock<Assembly>(MockBehavior.Strict);
    _services = new ServiceCollection();
    _services.AddSingleton(_daprClientMock.Object);
    _configuration = new DaprConfiguration();
  }

  [Fact]
  public void AddMessageBus_ShouldAddIServiceResolverOfIMessageBusAsSingleton()
  {
    // Arrange
    DaprInfrastructureBuilder sut = new DaprInfrastructureBuilder(_services, _configuration);

    // Act
    sut.AddMessageBus();

    // Assert
    IServiceCollection services = sut.Services;
    services.Where(x => x.ServiceType == typeof(IServiceResolver<IMessageBus>)).Should().HaveCount(1)
      .And.ContainSingle(x => x.Lifetime == ServiceLifetime.Singleton);
  }

  [Fact]
  public void AddMessageBus_ShouldAddIMessageBusAsSingleton_WhenConfigurationContainsDefaultBus()
  {
    // Arrange
    DaprInfrastructureBuilder sut = new DaprInfrastructureBuilder(_services, new DaprConfiguration
    {
      Service = new ServiceOptions
      {
        Messaging = new DaprMessagingOptions
        {
          DefaultBus = "defaultBus"
        }
      }
    });

    // Act
    sut.AddMessageBus();

    // Assert
    IServiceCollection services = sut.Services;
    services.Where(x => x.ServiceType == typeof(IMessageBus)).Should().HaveCount(1)
      .And.ContainSingle(x => x.Lifetime == ServiceLifetime.Singleton);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData(" ")]
  public void AddMessageBus_ShouldNotAddIMessageBus_WhenDefaultBusIsNullOrWhitespace(string defaultBus)
  {
    // Arrange
    DaprInfrastructureBuilder sut = new DaprInfrastructureBuilder(_services, new DaprConfiguration
    {
      Service = new ServiceOptions
      {
        Messaging = new DaprMessagingOptions
        {
          DefaultBus = defaultBus
        }
      }
    });

    // Act
    sut.AddMessageBus();

    // Assert
    IServiceCollection services = sut.Services;
    services.Where(x => x.ServiceType == typeof(IMessageBus)).Should().HaveCount(0);
  }

  [Fact]
  public void AddHandlers_ShouldRegisterIHandlerConfigurationAsSingleton()
  {
    // Arrange
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new[] { typeof(Message1Handler), typeof(Message2Handler) });
    _assemblyMock
      .Setup(x => x.GetHashCode())
      .Returns(1);
    DaprInfrastructureBuilder sut = new DaprInfrastructureBuilder(_services, _configuration);

    // Act
    sut.AddMessageHandlers(_assemblyMock.Object);

    // Assert
    IServiceCollection services = sut.Services;
    services.Where(x => x.ServiceType == typeof(IMessagingConfiguration)).Should().HaveCount(1)
      .And.ContainSingle(x => x.Lifetime == ServiceLifetime.Singleton);
  }

  [Fact]
  public void AddHandlers_ShouldRegisterAllHandlersAsScoped()
  {
    // Arrange
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new[] { typeof(Message1Handler), typeof(Message2Handler) });
    _assemblyMock
      .Setup(x => x.GetHashCode())
      .Returns(1);
    DaprInfrastructureBuilder sut = new DaprInfrastructureBuilder(_services, _configuration);

    // Act
    sut.AddMessageHandlers(_assemblyMock.Object);

    // Assert
    IServiceCollection services = sut.Services;
    services.Should()
      .ContainSingle(x => x.ServiceType == typeof(Message1Handler))
      .Which.Lifetime.Should().Be(ServiceLifetime.Scoped);
    services.Should()
      .ContainSingle(x => x.ServiceType == typeof(Message2Handler))
      .Which.Lifetime.Should().Be(ServiceLifetime.Scoped);
  }
}
