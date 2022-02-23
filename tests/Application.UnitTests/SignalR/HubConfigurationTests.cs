using CodeArchitects.Platform.Application.SignalR.Fakes;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace CodeArchitects.Platform.Application.SignalR;

public class HubConfigurationTests
{
  [Fact]
  public void Constructor_ShouldCreateHubMapCorrectly()
  {
    // Arrange

    // Act
    HubConfiguration sut = new HubConfiguration(new Assembly[] { HubAssembly.Instance });

    // Assert
    sut.HubMap.Should().HaveCount(2);
    sut.HubMap[HubAssembly.HubInterface1Type].Should().Be(HubAssembly.HubClass1Type);
    sut.HubMap[HubAssembly.HubInterface2Type].Should().Be(HubAssembly.HubClass2Type);
  }
}
