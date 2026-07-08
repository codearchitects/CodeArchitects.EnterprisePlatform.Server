using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

public partial class IdentityCollectionFactoryTests
{
  [Theory]
  [ModelData(CollectionKind.List)]
  public void CreateCollection_ShouldCreateIdentityList_WhenCollectionKindIsList(INavigationModel navigation, IEntityModel entity)
  {
    // Arrange
    ConcurrentDictionary<IEntityModel, IdentityCollectionFactory.Factory> factories = new();

    IdentityCollectionFactory sut = new(factories);

    // Act
    IIdentityCollection collection = sut.CreateCollection(navigation);

    // Assert
    collection.Should().BeOfType<IdentityList<Entity>>();
    factories.Should().HaveCount(1).And.ContainKey(entity);
  }

  [Theory]
  [ModelData(CollectionKind.HashSet)]
  public void CreateCollection_ShouldCreateIdentityList_WhenCollectionKindIsHashSet(INavigationModel navigation, IEntityModel entity)
  {
    // Arrange
    ConcurrentDictionary<IEntityModel, IdentityCollectionFactory.Factory> factories = new();

    IdentityCollectionFactory sut = new(factories);

    // Act
    IIdentityCollection collection = sut.CreateCollection(navigation);

    // Assert
    collection.Should().BeOfType<IdentityHashSet<Entity>>();
    factories.Should().HaveCount(1).And.ContainKey(entity);
  }
}
