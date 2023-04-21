using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

public class PredicateTemplateFactoryTests
{
  [Fact]
  public void BuildPredicateTemplate_ShouldBuildCorrectExpression_WhenTemplateVariableIsKey()
  {
    // Arrange
    PredicateTemplateFactory sut = new();
    EntityModel model = EntityModel.Create(typeof(EntityWithIdProperty));

    // Act
    Expression<Func<EntityWithIdProperty, bool>> template = sut.BuildPredicateTemplate<EntityWithIdProperty, Guid>(Models.EntityWithIdProperty);

    // Assert
    template.Should().BeEquivalentTo(PredicateTemplates.EntityWithIdPropertyKeyTemplate);
  }

  [Fact]
  public void BuildPredicateTemplate_ShouldBuildCorrectExpression_WhenTemplateVariableIsEntity()
  {
    // Arrange
    PredicateTemplateFactory sut = new();

    // Act
    Expression<Func<EntityWithIdProperty, bool>> template = sut.BuildPredicateTemplate<EntityWithIdProperty>(Models.EntityWithIdProperty);

    // Assert
    template.Should().BeEquivalentTo(PredicateTemplates.EntityWithIdPropertyEntityTemplate);
  }
}
