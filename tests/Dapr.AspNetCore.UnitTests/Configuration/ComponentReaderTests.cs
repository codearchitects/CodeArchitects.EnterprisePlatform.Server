using CodeArchitects.Platform.Dapr.AspNetCore.Components;
using FluentAssertions;
using Microsoft.Extensions.FileProviders;
using Xunit;
using static CodeArchitects.Platform.Dapr.AspNetCore.Configuration.ComponentReaderFixture;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Configuration;

public class ComponentReaderTests
{
  [Theory, ComponentData]
  public void FromFileProvider_ShouldReadComponents(IFileInfo file, ComponentSchema expectedComponent)
  {
    // Arrange
    ComponentReader sut = new ComponentReader();

    // Act
    ComponentSchema component = sut.FromFile(file);

    // Assert
    component.Should().BeEquivalentTo(expectedComponent);
  }
}
