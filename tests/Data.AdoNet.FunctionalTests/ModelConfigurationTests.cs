using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

namespace CodeArchitects.Platform.Data.AdoNet;

public partial class ModelConfigurationTests
{
  [Fact]
  public void ShouldCreateADataModelWithAOneToManyAggregation()
  {
    // Arrange
    WithAOneToManyAggregation.TestModelConfiguration sut = new();

    // Act
    DataModel dataModel = sut.CreateDataModel();

    // Assert
  }
}
