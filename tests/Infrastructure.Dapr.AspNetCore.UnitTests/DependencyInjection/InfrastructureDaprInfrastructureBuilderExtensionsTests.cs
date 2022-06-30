using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using CodeArchitects.Platform.Dapr.AspNetCore.DependencyInjection;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging.Fakes;
using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using CodeArchitects.Platform.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.DependencyInjection;

public class InfrastructureDaprInfrastructureBuilderExtensionsTests
{
  private readonly Mock<Assembly> _assemblyMock;
  private readonly Mock<IServiceCollection> _servicesMock;
  private readonly Mock<IDaprConfigurationBuilder> _configurationBuilderMock;
  private readonly IDaprInfrastructureBuilder _builder;

  public InfrastructureDaprInfrastructureBuilderExtensionsTests()
  {
    _assemblyMock = new Mock<Assembly>(MockBehavior.Strict);
    _servicesMock = new Mock<IServiceCollection>(MockBehavior.Loose);
    _configurationBuilderMock = new Mock<IDaprConfigurationBuilder>(MockBehavior.Strict);

    _builder = new FakeDaprInfrastructureBuilder(_servicesMock.Object, Mock.Of<IConfiguration>(), _configurationBuilderMock.Object, Mock.Of<ILogger>());
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData(" ")]
  public void AddMessageBus_ShouldAddResolverAndNotAddMessageBus_WhenDefaultBusIsNullOrWhitespace(string defaultBus)
  {
    // Arrange
    _configurationBuilderMock
      .Setup(x => x.Configuration.GetSection<DaprMessagingOptions>())
      .Returns(new DaprMessagingOptions { DefaultBus = defaultBus });

    // Act
    _builder.AddMessageBus();

    // Assert
    _servicesMock.Verify(x => x.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(IServiceResolver<IMessageBus>) && d.Lifetime == ServiceLifetime.Singleton)), Times.Once);
    _servicesMock.VerifyNoOtherCalls();
  }

  [Fact]
  public void AddMessageBus_ShouldAddResolverAndMessageBus_WhenConfigurationContainsDefaultBus()
  {
    // Arrange
    _configurationBuilderMock
      .Setup(x => x.Configuration.GetSection<DaprMessagingOptions>())
      .Returns(new DaprMessagingOptions { DefaultBus = "DefaultBus" });

    // Act
    _builder.AddMessageBus();

    // Assert
    _servicesMock.Verify(x => x.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(IServiceResolver<IMessageBus>) && d.Lifetime == ServiceLifetime.Singleton)), Times.Once);
    _servicesMock.Verify(x => x.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(IMessageBus) && d.Lifetime == ServiceLifetime.Singleton)), Times.Once);
    _servicesMock.VerifyNoOtherCalls();
  }

  [Fact]
  public void AddHandlers_ShouldRegisterHandlerServicesAndAllHandlers()
  {
    // Arrange
    _assemblyMock
      .Setup(x => x.GetTypes())
      .Returns(new[] { typeof(Message1Handler), typeof(Message2Handler) });
    _assemblyMock
      .Setup(x => x.GetHashCode())
      .Returns(1);
    _configurationBuilderMock
      .Setup(x => x.Configuration.GetSection<DaprMessagingOptions>())
      .Returns(new DaprMessagingOptions());

    // Act
    _builder.AddMessageHandlers(_assemblyMock.Object);

    // Assert
    _servicesMock.Verify(x => x.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(MessageHandlerMarkerService) && d.Lifetime == ServiceLifetime.Singleton)));
    _servicesMock.Verify(x => x.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(IMessagingConfiguration) && d.Lifetime == ServiceLifetime.Singleton)), Times.Once);
    _servicesMock.Verify(x => x.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(ITopicDelegateFactory) && d.Lifetime == ServiceLifetime.Singleton)), Times.Once);
    _servicesMock.Verify(x => x.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(Message1Handler) && d.Lifetime == ServiceLifetime.Scoped)), Times.Once);
    _servicesMock.Verify(x => x.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(Message2Handler) && d.Lifetime == ServiceLifetime.Scoped)), Times.Once);
    _servicesMock.VerifyNoOtherCalls();
  }
}
