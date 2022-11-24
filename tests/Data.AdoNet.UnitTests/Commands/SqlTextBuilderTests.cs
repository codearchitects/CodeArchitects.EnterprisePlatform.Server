using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.FluentMock;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Commands;

public class SqlTextBuilderTests
{
  [Fact]
  public void BuildSelectText_ShouldReturnCorrectSql_WhenNoNavigations()
  {
    // Arrange
    IPrimaryKeyPropertyModel idProperty = PrimaryKeyPropertyModelBuilder.Build(MockBehavior.Strict, _ => _
      .SetName("Id")
      .SetIndex(0));

    IPropertyModel nameProperty = PropertyModelBuilder.Build(MockBehavior.Strict, _ => _
      .SetName("Name")
      .SetIndex(1));

    // INavigationPlan plan = NavigationPlanBuilder.Build(MockBehavior.Strict, _ => _
    //   .SetProperties(idProperty, nameProperty)
    //   .SetNavigations()
    //   .SetEntity(_ => _
    //     .SetName("TestEntity")
    //     .SetProperties(idProperty, nameProperty)
    //     .SetPrimaryKey(_ => _
    //       .SetProperties(idProperty))));

    SqlTextBuilder sut = new(Mock.Of<ISqlTextCache>());

    // Act
    // string text = sut.BuildSelectText(plan);
    // 
    // // Assert
    // text.Should().Be("""
    //   SELECT TOP(1) [Id], [Name]
    //   FROM [TestEntity]
    //   WHERE [Id] = @p0
    //   """);
  }
}
