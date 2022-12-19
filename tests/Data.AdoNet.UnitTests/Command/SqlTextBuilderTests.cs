using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

public partial class SqlTextBuilderTests
{
  [Theory]
  [NoInclude]
  [IncludeOne]
  [IncludeTwo]
  [IncludeManyToManyAsLeaf]
  [IncludeAll]
  [IncludeOneInverseDepth1]
  [IncludeOneInverseDepth2]
  [IncludeManyToManyAsLeafAfterNode]
  [IncludeManyToManyAsNodeDepth1]
  [IncludeManyToManyAsNodeDepth2]
  internal void BuildSelectText_ShouldReturnCorrectSql(SqlTextBuilder sut, NavigationSpec spec, string expectedSql)
  {
    // Arrange

    // Act
    string sql = sut.BuildFindText(spec);

    // Assert
    sql.Should().Be(expectedSql);
  }
}
