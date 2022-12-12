using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet;

public partial class ModelConfigurationTests
{
  [Fact]
  public void ShouldCreateADataModelWithAOneToManyAggregation()
  {
    // Arrange
    WithAOneToManyAggregation.TestModelConfiguration sut = new();

    // Act
    IDataModel dataModel = sut.CreateDataModel();

    // Assert
  }

  [Fact]
  public void ShouldCreateADataModelWithAManyToManyComposition()
  {
    // Arrange
    WithAManyToManyComposition.TestModelConfiguration sut = new();

    // Act
    IDataModel dataModel = sut.CreateDataModel();

    // Assert
  }
}
