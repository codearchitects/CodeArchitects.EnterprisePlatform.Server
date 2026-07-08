using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.Model;

public class KeyModelTests
{
  [Fact]
  public void Create_ShouldCreateCorrectKeyModel()
  {
    // Arrange
    PropertyInfo propertyInfo = typeof(EntityWithIdProperty).GetProperty("Id")!;

    // Act
    KeyModel sut = KeyModel.Create(propertyInfo);

    // Assert
    sut.Should().BeEquivalentTo(KeyModels.IdProperty);
  }
}
