using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

public partial class RowReaderProviderTests
{
  [Theory]
  [ModelData]
  public void GetRowReader_ShouldCreateRowReader_WhenEntityHasSimpleKey(IEntityModel entity)
  {
    // Arrange
    ConcurrentDictionary<IEntityModel, RowReader> readers = new();

    RowReaderProvider sut = new(readers);

    // Act
    IRowReader reader = sut.GetRowReader(entity);

    // Assert
    reader.Should().BeAssignableTo<RowReader>();
    readers.Should().HaveCount(1).And.ContainKey(entity);
  }
}
