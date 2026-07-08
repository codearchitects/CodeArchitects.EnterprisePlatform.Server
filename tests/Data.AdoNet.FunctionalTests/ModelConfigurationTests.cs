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
    dataModel.Entities.Should().HaveCount(2)
      .And.ContainSingle(entity => entity.Type == typeof(WithAOneToManyIntraAggregate.Parent))
      .And.ContainSingle(entity => entity.Type == typeof(WithAOneToManyIntraAggregate.Child));
  }

  [Fact]
  public void ShouldCreateADataModelWithAManyToManyInterAggregate()
  {
    // Arrange
    WithAManyToManyInterAggregate.TestModelConfiguration sut = new();

    // Act
    IDataModel dataModel = sut.CreateDataModel();

    // Assert
    dataModel.Entities.Should().HaveCount(2)
      .And.ContainSingle(entity => entity.Type == typeof(WithAManyToManyInterAggregate.EntityA))
      .And.ContainSingle(entity => entity.Type == typeof(WithAManyToManyInterAggregate.EntityB));
  }
}
