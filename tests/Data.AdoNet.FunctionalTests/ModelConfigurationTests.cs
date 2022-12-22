using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet;

public partial class ModelConfigurationTests
{
  [Fact]
  public void ShouldCreateADataModelWithAOneToManyIntraAggregate()
  {
    // Arrange
    WithAOneToManyIntraAggregate.TestModelConfiguration sut = new();

    // Act
    IDataModel dataModel = sut.CreateDataModel();

    // Assert
  }

  [Fact]
  public void ShouldCreateADataModelWithAManyToManyInterAggregate()
  {
    // Arrange
    WithAManyToManyInterAggregate.TestModelConfiguration sut = new();

    // Act
    IDataModel dataModel = sut.CreateDataModel();

    // Assert
  }
}
