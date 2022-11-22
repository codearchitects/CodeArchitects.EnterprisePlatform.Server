using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Navigation.FluentMock;

namespace CodeArchitects.Platform.Data.AdoNet.Commands;

public class SqlTextBuilderTests
{
  [Fact]
  public void BuildSelectText_ShouldReturnCorrectSql_WhenNoNavigations()
  {
    // Arrange
    IPrimaryKeyPropertyModel idProperty = Mock.Of<IPrimaryKeyPropertyModel>(property => property.Name == "Id" && property.Index == 0, MockBehavior.Strict);

    IPropertyModel nameProperty = Mock.Of<IPrimaryKeyPropertyModel>(property => property.Name == "Name", MockBehavior.Strict);

    INavigationPlan plan = NavigationPlanBuilder.Build(MockBehavior.Strict, _ => _
      .SetProperties(idProperty, nameProperty)
      .SetNavigations()
      .SetEntity(_ => _
        .SetName("TestEntity")
        .SetProperties(idProperty, nameProperty)
        .SetPrimaryKey(_ => _
          .SetProperties(idProperty))));

    SqlTextBuilder sut = new();

    // Act
    string text = sut.BuildSelectText(plan);

    // Assert
    text.Should().Be("""
      SELECT TOP(1) [Id], [Name]
      FROM [TestEntity]
      WHERE [Id] = @p0
      """);
  }
}
