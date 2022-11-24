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

  [Fact]
  public void Test()
  {
    #region Mock
    INavigationSpec spec = NavigationSpecBuilder.Build(_ => _
      .SetNavigations(_ => _
        .Add(new NavigationLeaf(
          model: NavigationModelBuilder.Build(_ => _
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
      .SetEntity(_ => _
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

    SqlBuilder sut = new();

    var sql = sut.WriteSelectSql(spec);
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
    INavigationSpec spec = NavigationSpecBuilder.Build(_ => _
      .SetNavigations(_ => _
        .Add(new NavigationLeaf(
          model: NavigationModelBuilder.Build(_ => _
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
                  .SetColumnName("ParentId")))))))
        .Add(new NavigationNode(
          model: NavigationModelBuilder.Build(_ => _
            .SetId(2)
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
          navigations: new[]
          {
            new NavigationLeaf(
              model: NavigationModelBuilder.Build(_ => _
                .SetId(3)
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
      .SetEntity(_ => _
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

    SqlBuilder sut = new();

    var sql = sut.WriteSelectSql(spec);
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
