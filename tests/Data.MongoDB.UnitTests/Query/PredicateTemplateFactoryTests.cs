using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

public class PredicateTemplateFactoryTests
{
  [Fact]
  public void BuildPredicateTemplate_ShouldBuildCorrectExpression_WhenTemplateVariableIsKey()
  {
    // Arrange
    PredicateTemplateProvider sut = new();

    // Act
    LambdaExpression template = sut.GetFindPredicateTemplate<EntityWithIdProperty, Guid>(Models.EntityWithIdProperty);

    // Assert
    template.Should().BeEquivalentTo(PredicateTemplates.EntityWithIdPropertyKeyTemplate);
  }

  [Fact]
  public void BuildPredicateTemplate_ShouldBuildCorrectExpression_WhenTemplateVariableIsEntity()
  {
    // Arrange
    PredicateTemplateProvider sut = new();

    // Act
    LambdaExpression template = sut.GetFindPredicateTemplate<EntityWithIdProperty>(Models.EntityWithIdProperty);

    // Assert
    template.Should().BeEquivalentTo(PredicateTemplates.EntityWithIdPropertyEntityTemplate);
  }
}
