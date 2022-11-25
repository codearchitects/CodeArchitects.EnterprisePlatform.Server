using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Navigation.FluentMock;
using static CodeArchitects.Platform.Data.AdoNet.Fixtures.NavigationFixture.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

public partial class SqlTextBuilderTests
{
  [Fact]
  public void BuildSelectText_ShouldReturnCorrectSql_WhenIncludeOne()
  {
    // Arrange
    INavigationRoot root = NavigationRootBuilder.Build(_ => _
      .SetEntity(RootEntity)
      .SetNavigations(
        new NavigationLeaf(RootToChildANavigation)));

    string expectedSql = """
      SELECT t.[Id], t.[Name], t1.[Id] AS [Id_1], t1.[Name] AS [Name_1], t1.[RootId] AS [RootId_1]
      FROM (
      SELECT [Id], [Name]
      FROM [Root]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN [ChildA] AS t1 ON t.[Id] = t1.[RootId]
      """;

    SqlTextBuilder sut = new(Mock.Of<ISqlTextCache>());

    // Act
    string sql = sut.BuildSelectText(root);

    // Assert
    sql.Should().Be(expectedSql);
  }

  [Fact]
  public void BuildSelectText_ShouldReturnCorrectSql_WhenIncludeTwo()
  {
    // Arrange
    INavigationRoot root = NavigationRootBuilder.Build(_ => _
      .SetEntity(RootEntity)
      .SetNavigations(
        new NavigationLeaf(RootToChildANavigation),
        new NavigationLeaf(RootToChildBNavigation)));

    string expectedSql = """
      SELECT t.[Id], t.[Name], t1.[Id] AS [Id_1], t1.[Name] AS [Name_1], t1.[RootId] AS [RootId_1], t2.[Id] AS [Id_2], t2.[Name] AS [Name_2], t2.[RootId] AS [RootId_2]
      FROM (
      SELECT [Id], [Name]
      FROM [Root]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN [ChildA] AS t1 ON t.[Id] = t1.[RootId]
      LEFT JOIN [ChildB] AS t2 ON t.[Id] = t2.[RootId]
      """;

    SqlTextBuilder sut = new(Mock.Of<ISqlTextCache>());

    // Act
    string sql = sut.BuildSelectText(root);

    // Assert
    sql.Should().Be(expectedSql);
  }

  [Fact]
  public void BuildSelectText_ShouldReturnCorrectSql_WhenIncludeAll()
  {
    // Arrange
    INavigationRoot root = NavigationRootBuilder.Build(_ => _
      .SetEntity(RootEntity)
      .SetNavigations(
        new NavigationLeaf(RootToChildBNavigation),
        new NavigationNode(RootToChildANavigation, new INavigation[]
        {
          new NavigationNode(ChildAToChildDNavigation, new INavigation[]
          {
            new NavigationLeaf(ChildDToChildENavigation)
          }),
          new NavigationLeaf(ChildAToChildFNavigation)
        }),
        new NavigationLeaf(RootToChildCNavigation)));

    string expectedSql = """
      SELECT t.[Id], t.[Name], t2.[Id] AS [Id_2], t2.[Name] AS [Name_2], t2.[RootId] AS [RootId_2], t1.[Id_1], t1.[Name_1], t1.[RootId_1], t1.[Id_4], t1.[Name_4], t1.[ChildAId_4], t1.[Id_6], t1.[Name_6], t1.[ChildDId_6], t1.[Id_5], t1.[Name_5], t1.[ChildAId_5], t3.[Id] AS [Id_3], t3.[Name] AS [Name_3], t3.[RootId] AS [RootId_3]
      FROM (
      SELECT [Id], [Name]
      FROM [Root]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN [ChildB] AS t2 ON t.[Id] = t2.[RootId]
      LEFT JOIN (
      SELECT t.[Id] AS [Id_1], t.[Name] AS [Name_1], t.[RootId] AS [RootId_1], t4.[Id_4], t4.[Name_4], t4.[ChildAId_4], t4.[Id_6], t4.[Name_6], t4.[ChildDId_6], t5.[Id] AS [Id_5], t5.[Name] AS [Name_5], t5.[ChildAId] AS [ChildAId_5]
      FROM [ChildA] AS t
      LEFT JOIN (
      SELECT t.[Id] AS [Id_4], t.[Name] AS [Name_4], t.[ChildAId] AS [ChildAId_4], t6.[Id] AS [Id_6], t6.[Name] AS [Name_6], t6.[ChildDId] AS [ChildDId_6]
      FROM [ChildD] AS t
      LEFT JOIN [ChildE] AS t6 ON t.[Id] = t6.[ChildDId]
      ) AS t4 ON t.[Id] = t4.[ChildAId_4]
      LEFT JOIN [ChildF] AS t5 ON t.[Id] = t5.[ChildAId]
      ) AS t1 ON t.[Id] = t1.[RootId_1]
      LEFT JOIN [ChildC] AS t3 ON t.[Id] = t3.[RootId]
      """;

    SqlTextBuilder sut = new(Mock.Of<ISqlTextCache>());

    // Act
    string sql = sut.BuildSelectText(root);

    // Assert
    sql.Should().Be(expectedSql);
  }

  [Fact]
  public void BuildSelectText_ShouldReturnCorrectSql_WhenIncludeOneInverse()
  {
    // Arrange
    INavigationRoot root = NavigationRootBuilder.Build(_ => _
      .SetEntity(ChildAEntity)
      .SetNavigations(
        new NavigationLeaf(ChildAToRootNavigation)));

    string expectedSql = """
      SELECT t.[Id], t.[Name], t.[RootId], t7.[Id] AS [Id_7], t7.[Name] AS [Name_7]
      FROM (
      SELECT [Id], [Name], [RootId]
      FROM [ChildA]
      WHERE [Id] = @p0
      ) AS t
      LEFT JOIN [Root] AS t7 ON t.[RootId] = t7.[Id]
      """;

    SqlTextBuilder sut = new(Mock.Of<ISqlTextCache>());

    // Act
    string sql = sut.BuildSelectText(root);

    // Assert
    sql.Should().Be(expectedSql);
  }
}
