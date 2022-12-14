using CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components;

public class DaprComponentAccessorTests
{
  private readonly Mock<IComponentReader> _componentReaderMock;
  private readonly List<ComponentSchema> _components;

  private readonly DaprComponentAccessor _sut;

  public DaprComponentAccessorTests()
  {
    _componentReaderMock = new(MockBehavior.Strict);
    _components = new List<ComponentSchema>();

    _sut = new(_componentReaderMock.Object, Mock.Of<ILogger>(), _components);
  }

  [Fact]
  public void AddComponents_ShouldAddAllComponentsInFolder_WhenFolderAndFilesExist()
  {
    // Arrange
    ComponentSchema component1 = new();
    ComponentSchema component2 = new();

    Mock<IFileInfo> fileMock = new(MockBehavior.Strict);
    Mock<IDirectoryContents> directoryContentsMock = new(MockBehavior.Strict);
    Mock<IFileProvider> componentsFolderMock = new(MockBehavior.Strict);

    _componentReaderMock
      .SetupSequence(x => x.FromFile(It.IsAny<IFileInfo>()))
      .Returns(component1)
      .Returns(component2);

    componentsFolderMock
      .Setup(x => x.GetDirectoryContents("/"))
      .Returns(directoryContentsMock.Object);

    directoryContentsMock
      .Setup(x => x.Exists)
      .Returns(true);
    directoryContentsMock
      .Setup(x => x.GetEnumerator()).Returns(GetEnumerator());

    fileMock
      .Setup(x => x.Exists)
      .Returns(true);
    fileMock
      .Setup(x => x.IsDirectory)
      .Returns(false);

    IEnumerator<IFileInfo> GetEnumerator()
    {
      yield return fileMock.Object;
      yield return fileMock.Object;
    }

    // Act
    _sut.AddComponents(componentsFolderMock.Object);

    // Assert
    _components.Should().BeEquivalentTo(new[] { component1, component2 });
  }
}
