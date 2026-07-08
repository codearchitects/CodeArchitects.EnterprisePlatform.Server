using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;

namespace CodeArchitects.Platform.Application.SignalR;

using MapHubDelegate = ApplicationHubEndpointRouteBuilderExtensions.MapHubDelegate;

public class MapHubDelegateTests
{
  [Fact]
  public void Create_ShouldCreateMapHubDelegate_WhenHubTypeIsSubTypeOfHub()
  {
    // Arrange

    // Act
    MapHubDelegate mapHub = MapHubDelegate.Create(typeof(MyHub));

    // Assert
    mapHub.Should().NotBeNull();
  }

  private class MyHub : Hub { }
}
