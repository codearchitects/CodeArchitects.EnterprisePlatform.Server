using CodeArchitects.Platform.Application.SignalR.Fakes;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Application.SignalR;

public class ApplicationSignalRServerBuilderExtensionsTests
{
  private readonly ISignalRServerBuilder _builder;
  private readonly Mock<IServiceCollection> _serviceCollectionMock;

  public ApplicationSignalRServerBuilderExtensionsTests()
  {
    Mock<ISignalRServerBuilder> builderMock = new Mock<ISignalRServerBuilder>(behavior: MockBehavior.Strict);
    _serviceCollectionMock = new Mock<IServiceCollection>(behavior: MockBehavior.Strict);

    builderMock
      .Setup(x => x.Services)
      .Returns(_serviceCollectionMock.Object);

    _builder = builderMock.Object;
  }

  [Fact]
  public void AddHubs_ShouldAddHubConfigurationAsSingletonAndHubsAsTransient()
  {
    // Arrange
    List<ServiceDescriptor> descriptors = new List<ServiceDescriptor>();
    _serviceCollectionMock
      .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
      .Callback<ServiceDescriptor>(x => descriptors.Add(x));

    // Act
    _builder.AddHubs(HubAssembly.Instance);

    // Assert
    descriptors.Should().HaveCount(3)
      .And.ContainSingle(x => x.ServiceType == typeof(HubConfiguration) && x.Lifetime == ServiceLifetime.Singleton)
      .And.ContainSingle(x => x.ServiceType == HubAssembly.HubInterface1Type && x.ImplementationType == HubAssembly.HubClass1Type && x.Lifetime == ServiceLifetime.Transient)
      .And.ContainSingle(x => x.ServiceType == HubAssembly.HubInterface2Type && x.ImplementationType == HubAssembly.HubClass2Type && x.Lifetime == ServiceLifetime.Transient);
  }
}
