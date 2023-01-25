using CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;
using Microsoft.Extensions.FileProviders;
using static CodeArchitects.Platform.Dapr.AspNetCore.Components.ComponentReaderFixture;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components;

public class ComponentReaderTests
{
  [Theory, ComponentData]
  public void FromFileProvider_ShouldReadComponents(IFileInfo file, ComponentSchema expectedComponent)
  {
    // Arrange
    ComponentReader sut = new();

    // Act
    ComponentSchema component = sut.FromFile(file);

    // Assert
    component.Should().BeEquivalentTo(expectedComponent);
  }
}
