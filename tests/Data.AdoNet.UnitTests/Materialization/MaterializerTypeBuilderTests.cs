using CodeArchitects.Platform.Data.AdoNet.Fixtures;
using CodeArchitects.Platform.Data.AdoNet.Fixtures.Models;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

public partial class MaterializerTypeBuilderTests
{
  private static readonly MaterializerTypeBuilder s_typeBuilder = new(DynamicAssembly.CreateModule());
  private static readonly Type s_parentMaterializerType = s_typeBuilder.Build(WithNavigationOnParent.ParentModel);
  private static readonly Type s_childAMaterializerType = s_typeBuilder.Build(WithNavigationOnParent.ChildAModel);
  private static readonly Type s_childBMaterializerType = s_typeBuilder.Build(WithNavigationOnParent.ChildBModel);

  private readonly Materializer<WithNavigationOnParent.Parent, Guid> _sut;

  public MaterializerTypeBuilderTests()
  {
    Mock<IMaterializerHub> hubMock = new(MockBehavior.Strict);

    _sut = (Materializer<WithNavigationOnParent.Parent, Guid>)Activator.CreateInstance(s_parentMaterializerType, new object[] { hubMock.Object })!;
    var childAMaterializer = (Materializer<WithNavigationOnParent.ChildA, Guid>)Activator.CreateInstance(s_childAMaterializerType, new object[] { hubMock.Object })!;
    var childBMaterializer = (Materializer<WithNavigationOnParent.ChildB, int>)Activator.CreateInstance(s_childBMaterializerType, new object[] { hubMock.Object })!;

    hubMock
      .Setup(x => x.GetMaterializer<WithNavigationOnParent.ChildA, Guid>(WithNavigationOnParent.ChildAModel))
      .Returns(childAMaterializer);
    hubMock
      .Setup(x => x.GetMaterializer<WithNavigationOnParent.ChildB, int>(WithNavigationOnParent.ChildBModel))
      .Returns(childBMaterializer);
    hubMock
      .Setup(x => x.CreateHashSet<WithNavigationOnParent.ChildB>())
      .Returns(new IdentityHashSet<WithNavigationOnParent.ChildB>(new WithNavigationOnParent.ChildBEqualityComparer()));
  }

  [Fact]
  public void ReadEntity_ShouldReadParent()
  {
    // Arrange
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
    WithNavigationOnParent.Parent? materialized = _sut.ReadEntity(reader, ref offset, Array.Empty<INavigation>());

    // Assert
    materialized.Should().BeEquivalentTo(parent);
    offset.Should().Be(2);
  }

  [Fact]
  public void ReadEntity_ShouldReadParentWithOneToOneNavigation()
  {
    // Arrange
    int offset = 0;

    INavigation childANavigation = Mock.Of<INavigation>(nav =>
      nav.Model.Id == 1 &&
      nav.Target == WithNavigationOnParent.ChildAModel &&
      nav.Children == Array.Empty<INavigation>() &&
      nav.Model.Index == 0, MockBehavior.Strict);

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
    WithNavigationOnParent.Parent? materialized = _sut.ReadEntity(reader, ref offset, new[] { childANavigation });

    // Assert
    materialized.Should().BeEquivalentTo(parent);
  }

  [Fact]
  public void ReadEntity_ShouldReadParentWithOneToManyNavigation()
  {
    // Arrange
    int offset = 0;

    INavigation childBNavigation = Mock.Of<INavigation>(nav =>
      nav.Model.Id == 2 &&
      nav.Target == WithNavigationOnParent.ChildBModel &&
      nav.Children == Array.Empty<INavigation>() &&
      nav.Model.Index == 1, MockBehavior.Strict);

    WithNavigationOnParent.ChildB childB1 = new()
    {
      Id = 1,
      Name = "ChildB1"
    };
    WithNavigationOnParent.ChildB childB2 = new()
    {
      Id = 2,
      Name = "ChildB2"
    };
    WithNavigationOnParent.Parent parent = new()
    {
      Id = Guid.NewGuid(),
      Name = "Parent",
      ChildrenB = new List<WithNavigationOnParent.ChildB>()
      {
        childB1,
        childB2
      }
    };
    childB1.ParentId = parent.Id;
    childB2.ParentId = parent.Id;

    WithNavigationOnParent.FakeDbDataReader reader = new(new WithNavigationOnParent.FakeDbDataRow[]
    {
      new(parent, null, childB1),
      new(parent, null, childB2),
    });
    reader.Read();

    // Act
    _ = _sut.ReadEntity(reader, ref offset, new[] { childBNavigation });
    reader.Read();
    offset = 0;
    WithNavigationOnParent.Parent? materialized = _sut.ReadEntity(reader, ref offset, new[] { childBNavigation });

    // Assert
    materialized.Should().BeEquivalentTo(parent);
  }
}
