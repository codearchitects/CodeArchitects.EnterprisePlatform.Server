using CodeArchitects.Platform.Dapr.AspNetCore.Components;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Moq;
using static CodeArchitects.Platform.Dapr.AspNetCore.Configuration.DaprConfigurationFixture;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Configuration;

public class DaprConfigurationTests
{
  private readonly Mock<IComponentReader> _componentReaderMock;
  private readonly Mock<IConfigurationSection> _configurationMock;
  private readonly Dictionary<Type, object> _sections;
  private readonly List<ComponentSchema> _components;
  private readonly DaprConfiguration _sut;

  public DaprConfigurationTests()
  {
    _componentReaderMock = new(MockBehavior.Strict);
    _configurationMock = new(MockBehavior.Strict);
    _sections = new();
    _components = new();

    _sut = new DaprConfiguration(_componentReaderMock.Object, _configurationMock.Object, Mock.Of<ILogger>(MockBehavior.Loose), _sections, _components);
  }

  [Fact]
  public void AddSection_ShouldAddSectionValueWithItsTypeAsKey()
  {
    // Arrange
    string key = "key";
    _configurationMock
      .Setup(x => x.GetSection(key))
      .Returns(Mock.Of<IConfigurationSection>(MockBehavior.Loose));

    // Act
    Section section = _sut.AddSection<Section>(key);

    // Assert
    _sections.Should().HaveCount(1).And.ContainSingle(kvp => kvp.Key == typeof(Section) && kvp.Value == section);
  }

  [Fact]
  public void GetSection_ShouldReturnSectionWhenExists()
  {
    // Arrange
    Section expected = new Section();
    _sections[typeof(Section)] = expected;

    // Act
    Section? actual = _sut.GetSection<Section>();

    // Assert
    actual.Should().BeSameAs(expected);
  }

  [Fact]
  public void GetSection_ShouldReturnNullWhenItDoesNotExist()
  {
    // Arrange

    // Act
    Section? actual = _sut.GetSection<Section>();

    // Assert
    actual.Should().BeNull();
  }

  [Fact]
  public void AddComponents_ShouldAddAllComponentsInFolder_WhenFolderAndFilesExist()
  {
    // Arrange
    ComponentSchema component1 = new ComponentSchema();
    ComponentSchema component2 = new ComponentSchema();

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
