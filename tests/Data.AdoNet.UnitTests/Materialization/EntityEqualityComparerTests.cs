using CodeArchitects.Platform.Data.AdoNet.Fixtures;
using CodeArchitects.Platform.Data.AdoNet.Fixtures.Models;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

public partial class EntityEqualityComparerTests
{
  private readonly EntityEqualityComparerTypeBuilder _sut;

  public EntityEqualityComparerTests()
  {
    _sut = new(DynamicAssembly.CreateModule());
  }

  [Fact]
  public void Equals_ShouldReturnTrue_WhenEntitiesHaveSameSimpleKey()
  {
    // Arrange
    WithDifferentPrimaryKeys.SimpleEntity entity1 = new()
    {
      Id = 1,
      Name = "Name1"
    };
    WithDifferentPrimaryKeys.SimpleEntity entity2 = new()
    {
      Id = 1,
      Name = "Name2"
    };

    // Act
    Type type = _sut.Build(WithDifferentPrimaryKeys.CreateSimpleEntityModel(false));
    var comparer = (IEqualityComparer<WithDifferentPrimaryKeys.SimpleEntity>)Activator.CreateInstance(type)!;
    bool areEqual = comparer.Equals(entity1, entity2);

    // Assert
    areEqual.Should().BeTrue();
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenEntitiesHaveDifferentSimpleKey()
  {
    // Arrange
    WithDifferentPrimaryKeys.SimpleEntity entity1 = new()
    {
      Id = 1,
      Name = "Name1"
    };
    WithDifferentPrimaryKeys.SimpleEntity entity2 = new()
    {
      Id = 2,
      Name = "Name2"
    };

    // Act
    Type type = _sut.Build(WithDifferentPrimaryKeys.CreateSimpleEntityModel(false));
    var comparer = (IEqualityComparer<WithDifferentPrimaryKeys.SimpleEntity>)Activator.CreateInstance(type)!;
    bool areEqual = comparer.Equals(entity1, entity2);

    // Assert
    areEqual.Should().BeFalse();
  }

  [Fact]
  public void Equals_ShouldReturnTrue_WhenEntitiesHaveSameCompositeKey()
  {
    // Arrange
    Guid id1 = Guid.NewGuid();
    WithDifferentPrimaryKeys.CompositeEntity entity1 = new()
    {
      Id1 = id1,
      Id2 = 3,
      Name = "Name1"
    };
    WithDifferentPrimaryKeys.CompositeEntity entity2 = new()
    {
      Id1 = id1,
      Id2 = 3,
      Name = "Name2"
    };

    // Act
    Type type = _sut.Build(WithDifferentPrimaryKeys.CreateCompositeEntityModel(false));
    var comparer = (IEqualityComparer<WithDifferentPrimaryKeys.CompositeEntity>)Activator.CreateInstance(type)!;
    bool areEqual = comparer.Equals(entity1, entity2);

    // Assert
    areEqual.Should().BeTrue();
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenEntitiesHaveDifferentCompositeKey()
  {
    // Arrange
    Guid id1 = Guid.NewGuid();
    WithDifferentPrimaryKeys.CompositeEntity entity1 = new()
    {
      Id1 = id1,
      Id2 = 3,
      Name = "Name1"
    };
    WithDifferentPrimaryKeys.CompositeEntity entity2 = new()
    {
      Id1 = id1,
      Id2 = 4,
      Name = "Name2"
    };

    // Act
    Type type = _sut.Build(WithDifferentPrimaryKeys.CreateCompositeEntityModel(false));
    var comparer = (IEqualityComparer<WithDifferentPrimaryKeys.CompositeEntity>)Activator.CreateInstance(type)!;
    bool areEqual = comparer.Equals(entity1, entity2);

    // Assert
    areEqual.Should().BeFalse();
  }
}
