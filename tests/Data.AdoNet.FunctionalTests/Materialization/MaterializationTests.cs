using CodeArchitects.Platform.Data.AdoNet.Fixtures;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

public partial class MaterializationTests
{
  private readonly Materializer _sut;

  public MaterializationTests()
  {
    _sut = new(IdentityCollectionFactory.Create(), RowReaderProvider.Create());
  }

  [Fact]
  public async Task ReadEntity_ShouldReadRoot()
  {
    // Arrange
    Parent p = Parent.One();
    FakeDbDataReader reader = new(new object?[,]
    {
      { p.Id, p.Name }
    });

    var spec = new NavigationSpec<Parent, Guid>(Model.ParentModel, Array.Empty<INavigation>());

    // Act
    Parent? materialized = await _sut.ReadEntityAsync(reader, spec, CancellationToken.None);

    // Assert
    materialized.Should().BeEquivalentTo(p);
  }

  [Fact]
  public async Task ReadEntity_ShouldReadRootAndOTO()
  {
    // Arrange
    Parent p = Parent.One();
    ChildC c = ChildC.One(p.Id);
    p.ChildC = c;
    FakeDbDataReader reader = new(new object?[,]
    {
      { p.Id, p.Name, c.Id, c.Name, c.ParentId }
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
    materialized.Should().BeEquivalentTo(p);
  }

  [Fact]
  public async Task ReadEntity_ShouldReadRootAndOTM()
  {
    // Arrange
    Parent p = Parent.One();
    List<ChildA> a = ChildA.Many(2, p.Id);
    p.ChildrenA = a;
    FakeDbDataReader reader = new(new object?[,]
    {
      { p.Id, p.Name, a[0].Id, a[0].Name, a[0].ParentId },
      { p.Id, p.Name, a[1].Id, a[1].Name, a[1].ParentId }
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
    materialized.Should().BeEquivalentTo(p);
  }

  [Fact]
  public async Task ReadEntity_ShouldReadRootAndOTOAndOTM()
  {
    // Arrange
    Parent p = Parent.One();
    ChildC c = ChildC.One(p.Id);
    List<ChildA> childrenA = ChildA.Many(2, p.Id);
    p.ChildC = c;
    p.ChildrenA = childrenA;
    FakeDbDataReader reader = new(new object?[,]
    {
      { p.Id, p.Name, c.Id, c.Name, c.ParentId, childrenA[0].Id, childrenA[0].Name, childrenA[0].ParentId },
      { p.Id, p.Name, c.Id, c.Name, c.ParentId, childrenA[1].Id, childrenA[1].Name, childrenA[1].ParentId }
    });
    var spec = new NavigationSpec<Parent, Guid>(Model.ParentModel, new INavigation[]
    {
      Mock.Of<INavigation>(nav =>
        nav.Model == Model.ParentToChildCNavigation &&
        nav.Target == Model.ChildCModel &&
        nav.Children == Array.Empty<INavigation>(), MockBehavior.Strict),
      Mock.Of<INavigation>(nav =>
        nav.Model == Model.ParentToChildANavigation &&
        nav.Target == Model.ChildAModel &&
        nav.Children == Array.Empty<INavigation>(), MockBehavior.Strict)
    });

    // Act
    Parent? materialized = await _sut.ReadEntityAsync(reader, spec, CancellationToken.None);

    // Assert
    materialized.Should().BeEquivalentTo(p);
  }

  [Fact]
  public async Task ReadEntity_ShouldReadRootAnd2OTM()
  {
    // Arrange
    Parent p = Parent.One();
    List<ChildA> a = ChildA.Many(2, p.Id);
    List<ChildB> b = ChildB.Many(3, p.Id);
    p.ChildrenA = a;
    p.ChildrenB = b.ToHashSet();
    FakeDbDataReader reader = new(new object?[,]
    {
      { p.Id, p.Name, a[0].Id, a[0].Name, a[0].ParentId, b[0].Id, b[0].Name, b[0].ParentId },
      { p.Id, p.Name, a[0].Id, a[0].Name, a[0].ParentId, b[1].Id, b[1].Name, b[1].ParentId },
      { p.Id, p.Name, a[0].Id, a[0].Name, a[0].ParentId, b[2].Id, b[2].Name, b[2].ParentId },
      { p.Id, p.Name, a[1].Id, a[1].Name, a[1].ParentId, b[0].Id, b[0].Name, b[0].ParentId },
      { p.Id, p.Name, a[1].Id, a[1].Name, a[1].ParentId, b[1].Id, b[1].Name, b[1].ParentId },
      { p.Id, p.Name, a[1].Id, a[1].Name, a[1].ParentId, b[2].Id, b[2].Name, b[2].ParentId }
    });
    var spec = new NavigationSpec<Parent, Guid>(Model.ParentModel, new INavigation[]
    {
      Mock.Of<INavigation>(nav =>
        nav.Model == Model.ParentToChildANavigation &&
        nav.Target == Model.ChildAModel &&
        nav.Children == Array.Empty<INavigation>(), MockBehavior.Strict),
      Mock.Of<INavigation>(nav =>
        nav.Model == Model.ParentToChildBNavigation &&
        nav.Target == Model.ChildBModel &&
        nav.Children == Array.Empty<INavigation>(), MockBehavior.Strict)
    });

    // Act
    Parent? materialized = await _sut.ReadEntityAsync(reader, spec, CancellationToken.None);

    // Assert
    materialized.Should().BeEquivalentTo(p);
  }

  [Fact]
  public async Task ReadEntity_ShouldReadRootAnd2OTMDepth2()
  {
    // Arrange
    Parent p = Parent.One();
    List<ChildA> a = ChildA.Many(2, p.Id);
    List<ChildD> d0 = ChildD.Many(2, a[0].Id);
    List<ChildD> d1 = ChildD.Many(2, a[1].Id);
    List<ChildB> b = ChildB.Many(3, p.Id);
    p.ChildrenA = a;
    p.ChildrenB = b.ToHashSet();
    a[0].ChildrenD = d0;
    a[1].ChildrenD = d1;
    FakeDbDataReader reader = new(new object?[,]
    {
      { p.Id, p.Name, a[0].Id, a[0].Name, a[0].ParentId, d0[0].Id, d0[0].Name, d0[0].ChildAId, b[0].Id, b[0].Name, b[0].ParentId },
      { p.Id, p.Name, a[0].Id, a[0].Name, a[0].ParentId, d0[0].Id, d0[0].Name, d0[0].ChildAId, b[1].Id, b[1].Name, b[1].ParentId },
      { p.Id, p.Name, a[0].Id, a[0].Name, a[0].ParentId, d0[0].Id, d0[0].Name, d0[0].ChildAId, b[2].Id, b[2].Name, b[2].ParentId },
      { p.Id, p.Name, a[0].Id, a[0].Name, a[0].ParentId, d0[1].Id, d0[1].Name, d0[1].ChildAId, b[1].Id, b[0].Name, b[0].ParentId },
      { p.Id, p.Name, a[0].Id, a[0].Name, a[0].ParentId, d0[1].Id, d0[1].Name, d0[1].ChildAId, b[1].Id, b[1].Name, b[1].ParentId },
      { p.Id, p.Name, a[0].Id, a[0].Name, a[0].ParentId, d0[1].Id, d0[1].Name, d0[1].ChildAId, b[1].Id, b[2].Name, b[2].ParentId },
      { p.Id, p.Name, a[1].Id, a[1].Name, a[1].ParentId, d1[0].Id, d1[0].Name, d1[0].ChildAId, b[0].Id, b[0].Name, b[0].ParentId },
      { p.Id, p.Name, a[1].Id, a[1].Name, a[1].ParentId, d1[0].Id, d1[0].Name, d1[0].ChildAId, b[1].Id, b[1].Name, b[1].ParentId },
      { p.Id, p.Name, a[1].Id, a[1].Name, a[1].ParentId, d1[0].Id, d1[0].Name, d1[0].ChildAId, b[2].Id, b[2].Name, b[2].ParentId },
      { p.Id, p.Name, a[1].Id, a[1].Name, a[1].ParentId, d1[1].Id, d1[1].Name, d1[1].ChildAId, b[1].Id, b[0].Name, b[0].ParentId },
      { p.Id, p.Name, a[1].Id, a[1].Name, a[1].ParentId, d1[1].Id, d1[1].Name, d1[1].ChildAId, b[1].Id, b[1].Name, b[1].ParentId },
      { p.Id, p.Name, a[1].Id, a[1].Name, a[1].ParentId, d1[1].Id, d1[1].Name, d1[1].ChildAId, b[1].Id, b[2].Name, b[2].ParentId }
    });
    var spec = new NavigationSpec<Parent, Guid>(Model.ParentModel, new INavigation[]
    {
      Mock.Of<INavigation>(nav =>
        nav.Model == Model.ParentToChildANavigation &&
        nav.Target == Model.ChildAModel &&
        nav.Children == new INavigation[]
        {
          Mock.Of<INavigation>(nav =>
            nav.Model == Model.ChildAToChildDNavigation &&
            nav.Target == Model.ChildDModel &&
            nav.Children == Array.Empty<INavigation>(), MockBehavior.Strict)
        }, MockBehavior.Strict),
      Mock.Of<INavigation>(nav =>
        nav.Model == Model.ParentToChildBNavigation &&
        nav.Target == Model.ChildBModel &&
        nav.Children == Array.Empty<INavigation>(), MockBehavior.Strict)
    });

    // Act
    Parent? materialized = await _sut.ReadEntityAsync(reader, spec, CancellationToken.None);

    // Assert
    materialized.Should().BeEquivalentTo(p);
  }
}
