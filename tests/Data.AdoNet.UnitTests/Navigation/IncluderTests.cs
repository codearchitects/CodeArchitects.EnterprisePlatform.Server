using CodeArchitects.Platform.Data.AdoNet.Commands;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.FluentMock;
using CodeArchitects.Platform.Data.AdoNet.Navigation.FluentMock;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

public class IncluderTests
{
  class Parent
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public ChildA? ChildA { get; set; }
    public ChildB? ChildB { get; set; }
  }

  class ChildA
  {
    public int Id { get; set; }
    public int ParentId { get; set; }
  }

  class ChildB
  {
    public int Id { get; set; }
    public int ParentId { get; set; }
    public ChildC? ChildC { get; set; }
  }

  class ChildC
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public int ChildBId { get; set; }
  }

  record NavigationLeaf(INavigationModel Model) : INavigationLeaf
  {
    public int Index => Model.Id;

    public IEntityModel Target => Model.To;

    public void Accept<TVisitor>(in TVisitor visitor) where TVisitor : INavigationVisitor
    {
      visitor.VisitLeaf(this);
    }

    public void Accept<TVisitor, TState>(in TVisitor visitor, in TState state) where TVisitor : INavigationVisitor<TState>
    {
      visitor.VisitLeaf(this, in state);
    }
  }

  record NavigationNode(INavigationModel Model, IReadOnlyList<INavigation> Children) : INavigationNode
  {
    public int Index => Model.Id;

    public IEntityModel Target => Model.To;

    public void Accept<TVisitor>(in TVisitor visitor) where TVisitor : INavigationVisitor
    {
      visitor.VisitNode(this);
    }

    public void Accept<TVisitor, TState>(in TVisitor visitor, in TState state) where TVisitor : INavigationVisitor<TState>
    {
      visitor.VisitNode(this, in state);
    }
  }

  [Fact]
  public void Test()
  {
    #region Mock
    INavigationRoot spec = NavigationRootBuilder.Build(_ => _
      .SetChildren(_ => _
        .Add(new NavigationLeaf(
          Model: NavigationModelBuilder.Build(_ => _
            .SetId(1)
            .SetIsOnDependent(false)
            .SetTo(_ => _
              .SetTableName("ChildA")
              .SetProperties(_ => _
                .Add(_ => _
                  .SetColumnName("Id"))
                .Add(_ => _
                  .SetColumnName("ParentId"))))
            .SetKeys(_ => _
              .Add(_ => _
                .SetFromProperty(_ => _
                  .SetColumnName("Id"))
                .SetToProperty(_ => _
                  .SetColumnName("ParentId"))))))))
      .SetTarget(_ => _
        .SetTableName("Parent")
        .SetProperties(_ => _
          .Add(_ => _
            .SetColumnName("Id"))
          .Add(_ => _
            .SetColumnName("Name")))
        .SetPrimaryKey(_ => _
          .SetProperties(_ => _
            .Add(_ => _
              .SetColumnName("Id")
              .SetIndex(0))))));
    #endregion

    SqlTextBuilder sut = new(Mock.Of<ISqlTextCache>());

    var sql = sut.BuildSelectText(spec);
  }

  [Fact]
  public void Test2()
  {
    // IRepository<Parent, int> repo = null!;
    // 
    // repo.FindAsync(2, _ => _
    //   .Include(parent => parent.ChildA)
    //   .Include(parent => parent.ChildB, _ => _
    //     .Include(childB => childB.ChildC)));

    #region Mock
    INavigationRoot spec = NavigationRootBuilder.Build(_ => _
      .SetChildren(_ => _
        .Add(new NavigationLeaf(
          Model: NavigationModelBuilder.Build(_ => _
            .SetId(12)
            .SetIsOnDependent(false)
            .SetTo(_ => _
              .SetTableName("ChildA")
              .SetProperties(_ => _
                .Add(_ => _
                  .SetColumnName("Id"))
                .Add(_ => _
                  .SetColumnName("ParentId"))))
            .SetKeys(_ => _
              .Add(_ => _
                .SetFromProperty(_ => _
                  .SetColumnName("Id"))
                .SetToProperty(_ => _
                  .SetColumnName("ParentId")))))))
        .Add(new NavigationNode(
          Model: NavigationModelBuilder.Build(_ => _
            .SetId(93)
            .SetIsOnDependent(false)
            .SetTo(_ => _
              .SetTableName("ChildB")
              .SetProperties(_ => _
                .Add(_ => _
                  .SetColumnName("Id"))
                .Add(_ => _
                  .SetColumnName("ParentId"))))
            .SetKeys(_ => _
              .Add(_ => _
                .SetFromProperty(_ => _
                  .SetColumnName("Id"))
                .SetToProperty(_ => _
                  .SetColumnName("ParentId"))))),
          Children: new[]
          {
            new NavigationLeaf(
              Model: NavigationModelBuilder.Build(_ => _
                .SetId(42)
                .SetIsOnDependent(false)
                .SetTo(_ => _
                  .SetTableName("ChildC")
                  .SetProperties(_ => _
                    .Add(_ => _
                      .SetColumnName("Id"))
                    .Add(_ => _
                      .SetColumnName("Name"))
                    .Add(_ => _
                      .SetColumnName("ChildBId"))))
                .SetKeys(_ => _
                  .Add(_ => _
                    .SetFromProperty(_ => _
                      .SetColumnName("Id"))
                    .SetToProperty(_ => _
                      .SetColumnName("ChildBId"))))))
          })))
      .SetTarget(_ => _
        .SetTableName("Parent")
        .SetProperties(_ => _
          .Add(_ => _
            .SetColumnName("Id"))
          .Add(_ => _
            .SetColumnName("Name")))
        .SetPrimaryKey(_ => _
          .SetProperties(_ => _
            .Add(_ => _
              .SetColumnName("Id")
              .SetIndex(0))))));
    #endregion

    SqlTextBuilder sut = new(Mock.Of<ISqlTextCache>());

    var sql = sut.BuildSelectText(spec);
  }


  // Parent (Id, Name)
  // |
  // |- ChildA (Id, ParentId)
  // |
  // |- ChildB (Id, ParentId)
  //    |
  //    |- ChildC (Id, Name, ChildBId)
  // 
  // 
  // SELECT ???
  // FROM (
  //   SELECT [Id], [Name]
  //   FROM [Parent]
  //   WHERE [Id] = @p0
  // ) AS t
  // LEFT JOIN [ChildA] AS t0 ON t.[Id] = t0.[ParentId]
  // LEFT JOIN (
  //   SELECT ???
  //   FROM [ChildB] AS t
  //   LEFT JOIN [ChildC] AS t0 ON t.[Id] = t0.[ChildBId]
  // ) AS t1 ON t.[Id] = t1.[ParentId]
}
