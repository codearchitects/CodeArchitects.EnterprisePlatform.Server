using CodeArchitects.Platform.Data.AdoNet.Fixtures;
using CodeArchitects.Platform.Data.AdoNet.Fixtures.Models;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

public class MaterializerTypeBuilderTests
{
  private readonly MaterializerTypeBuilder _sut;

  public MaterializerTypeBuilderTests()
  {
    _sut = new(DynamicAssembly.CreateModule());
  }

  [Fact]
  public void ReadEntity_ShouldReadParent()
  {
    // Arrange
    IMaterializerHub hub = Mock.Of<IMaterializerHub>(MockBehavior.Strict);
    int offset = 0;
    WithNavigationOnParent.Parent parent = new()
    {
      Id = Guid.NewGuid(),
      Name = "Parent"
    };
    WithNavigationOnParent.FakeDbDataReader reader = new(new WithNavigationOnParent.FakeDbDataRow[]
    {
      new(parent, null, null)
    });
    reader.Read();

    // Act
    Type type = _sut.Build(WithNavigationOnParent.ParentModel);
    var materializer = (Materializer<WithNavigationOnParent.Parent, Guid>)Activator.CreateInstance(type, new object[] { hub })!;
    WithNavigationOnParent.Parent? materialized = materializer.ReadEntity(reader, ref offset, Array.Empty<INavigation>());

    // Assert
    materialized.Should().NotBeNull().And.BeEquivalentTo(parent);
    offset.Should().Be(2);
  }

  [Fact]
  public void ReadEntity_ShouldReadParentWithOneToOneNavigation()
  {
    // Arrange
    IMaterializerHub hub = Mock.Of<IMaterializerHub>(MockBehavior.Strict);
    int offset = 0;

    INavigation childANavigation = Mock.Of<INavigation>(nav =>
      nav.Id == 2 &&
      nav.Model.Index == 2);

    WithNavigationOnParent.ChildA childA = new()
    {
      Id = Guid.NewGuid(),
      Name = "ChildA"
    };
    WithNavigationOnParent.Parent parent = new()
    {
      Id = Guid.NewGuid(),
      Name = "Parent",
      ChildA = childA
    };
    childA.ParentId = parent.Id;

    WithNavigationOnParent.FakeDbDataReader reader = new(new WithNavigationOnParent.FakeDbDataRow[]
    {
      new(parent, childA, null)
    });
    reader.Read();

    // Act
    Type type = _sut.Build(WithNavigationOnParent.ParentModel);
    var materializer = (Materializer<WithNavigationOnParent.Parent, Guid>)Activator.CreateInstance(type, new object[] { hub })!;
    WithNavigationOnParent.Parent? materialized = materializer.ReadEntity(reader, ref offset, new[] { childANavigation });

    // Assert
    materialized.Should().NotBeNull().And.BeEquivalentTo(parent);
  }
}
