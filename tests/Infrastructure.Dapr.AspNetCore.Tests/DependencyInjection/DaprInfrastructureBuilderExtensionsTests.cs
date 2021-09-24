using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Tests;
using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using CodeArchitects.Platform.Infrastructure.Messaging;
using Dapr.Client;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.DependencyInjection
{
  public class DaprInfrastructureBuilderExtensionsTests
  {
    private readonly Mock<DaprClient> _daprClientMock;
    private readonly ServiceCollection _services;

    public DaprInfrastructureBuilderExtensionsTests()
    {
      _daprClientMock = new Mock<DaprClient>(behavior: MockBehavior.Strict);
      _services = new ServiceCollection();
      _services.AddSingleton(_daprClientMock.Object);
    }

    [Fact]
    public void AddMessageBus_ShouldAddIServiceResolverOfIMessageBusAsSingleton()
    {
      // Arrange
      DaprInfrastructureBuilder sut = new DaprInfrastructureBuilder(_services);

      // Act
      sut.AddMessageBus();

      // Assert
      IServiceCollection services = sut.Services;
      services.Where(x => x.ServiceType == typeof(IServiceResolver<IMessageBus>)).Should()
        .HaveCount(1).And
        .ContainSingle(x => x.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddMessageBus_ShouldAddIServiceResolverOfIMessageBusOfDaprMetadataAsSingleton()
    {
      // Arrange
      DaprInfrastructureBuilder sut = new DaprInfrastructureBuilder(_services);

      // Act
      sut.AddMessageBus();

      // Assert
      IServiceCollection services = sut.Services;
      services.Where(x => x.ServiceType == typeof(IServiceResolver<IMessageBus<DaprMetadata>>)).Should()
        .HaveCount(1).And
        .ContainSingle(x => x.Lifetime == ServiceLifetime.Singleton);
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
      services.Where(x => x.ServiceType == typeof(IMessageBus)).Should()
        .HaveCount(1).And
        .ContainSingle(x => x.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddMessageBus_ShouldAddIMessageBusOfDaprMetadataAsSingleton_WhenConfigurationContainsDefaultBus()
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
      services.Where(x => x.ServiceType == typeof(IMessageBus<DaprMetadata>)).Should()
        .HaveCount(1).And
        .ContainSingle(x => x.Lifetime == ServiceLifetime.Singleton);
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
      services.Where(x => x.ServiceType == typeof(IMessageBus<DaprMetadata>)).Should().HaveCount(0);
    }

    [Fact]
    public void AddHandlers_ShouldRegisterIHandlerConfigurationAsSingleton()
    {
      // Arrange
      DaprInfrastructureBuilder sut = new DaprInfrastructureBuilder(_services);

      // Act
      sut.AddMessageHandlers(HandlerAssembly.Valid.Instance);

      // Assert
      IServiceCollection services = sut.Services;
      services.Where(x => x.ServiceType == typeof(IMessageHandlerConfiguration)).Should()
        .HaveCount(1).And
        .ContainSingle(x => x.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddHandlers_ShouldRegisterAllHandlersAsScoped()
    {
      // Arrange
      DaprInfrastructureBuilder sut = new DaprInfrastructureBuilder(_services);

      // Act
      sut.AddMessageHandlers(HandlerAssembly.Valid.Instance);

      // Assert
      IServiceCollection services = sut.Services;
      foreach (Type type in HandlerAssembly.Valid.HandlerTypes)
      {
        services.Where(x => x.ServiceType == type).Should()
          .HaveCount(1).And
          .ContainSingle(x => x.Lifetime == ServiceLifetime.Scoped);
      }
    }
  }
}
