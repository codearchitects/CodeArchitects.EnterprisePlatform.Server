using CodeArchitects.Platform.Data.AdoNet.Fixtures;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Configuration;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

public partial class MaterializationTests
{
  private readonly Materializer _sut;

  public MaterializationTests()
  {
    _sut = new(IdentityCollectionFactory.Create(), RowReaderProvider.Create());
  }

  [Fact]
  public async Task ReadEntity_ShouldReadParent()
  {
    // Arrange
    Parent parent = Parent.One();
    FakeDbDataReader reader = new(new object?[,]
    {
      { parent.Id, parent.Name }
    });

    var spec = new NavigationSpec<Parent, Guid>(Model.ParentModel, Array.Empty<INavigation>());

    // Act
    Parent? materialized = await _sut.ReadEntityAsync(reader, spec, CancellationToken.None);

    // Assert
    materialized.Should().BeEquivalentTo(parent);
  }

  [Fact]
  public async Task ReadEntity_ShouldReadParentAndOneToOne()
  {
    // Arrange
    Parent parent = Parent.One();
    ChildC childC = ChildC.One(parent.Id);
    parent.ChildC = childC;
    FakeDbDataReader reader = new(new object?[,]
    {
      { parent.Id, parent.Name, childC.Id, childC.Name, childC.ParentId }
    });

    var spec = new NavigationSpec<Parent, Guid>(Model.ParentModel, new INavigation[]
    {
      Mock.Of<INavigation>(nav =>
        nav.Model == Model.ParentToChildCNavigation &&
        nav.Target == Model.ChildCModel &&
        nav.Children == Array.Empty<INavigation>(), MockBehavior.Strict)
    });

    // Act
    Parent? materialized = await _sut.ReadEntityAsync(reader, spec, CancellationToken.None);

    // Assert
    materialized.Should().BeEquivalentTo(parent);
  }

  [Fact]
  public async Task ReadEntity_ShouldReadParentAndOneToMany()
  {
    // Arrange
    Parent parent = Parent.One();
    List<ChildA> childrenA = ChildA.Many(2, parent.Id);
    parent.ChildrenA = childrenA;
    FakeDbDataReader reader = new(new object?[,]
    {
      { parent.Id, parent.Name, childrenA[0].Id, childrenA[0].Name, childrenA[0].ParentId },
      { parent.Id, parent.Name, childrenA[1].Id, childrenA[1].Name, childrenA[1].ParentId }
    });

    var spec = new NavigationSpec<Parent, Guid>(Model.ParentModel, new INavigation[]
    {
      Mock.Of<INavigation>(nav =>
        nav.Model == Model.ParentToChildANavigation &&
        nav.Target == Model.ChildAModel &&
        nav.Children == Array.Empty<INavigation>(), MockBehavior.Strict)
    });

    // Act
    Parent? materialized = await _sut.ReadEntityAsync(reader, spec, CancellationToken.None);

    // Assert
    materialized.Should().BeEquivalentTo(parent);
  }
}
