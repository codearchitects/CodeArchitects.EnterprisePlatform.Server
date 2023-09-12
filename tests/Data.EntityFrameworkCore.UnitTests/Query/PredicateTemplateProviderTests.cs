using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

public class PredicateTemplateProviderTests
{
  [Fact]
  public void BuildFindPredicateTemplate_ShouldBuildCorrectExpression_WhenEntityHasSimplePropertyKey()
  {
    // Arrange
    PredicateTemplateProvider sut = new();

    // Act
    LambdaExpression template = sut.GetFindPredicateTemplate<EntityWithSimplePropertyKey, int>(Models.SimplePropertyKeyModel);

    // Assert
    template.Should().BeEquivalentTo(FindPredicateTemplates.SimplePropertyKeyTemplate);
  }

  [Fact]
  public void BuildFindPredicateTemplate_ShouldBuildCorrectExpression_WhenEntityHasSimpleFieldKey()
  {
    // Arrange
    PredicateTemplateProvider sut = new();

    // Act
    LambdaExpression template = sut.GetFindPredicateTemplate<EntityWithSimpleFieldKey, int>(Models.SimpleFieldKeyModel);

    // Assert
    template.Should().BeEquivalentTo(FindPredicateTemplates.SimpleFieldKeyTemplate);
  }

  [Fact]
  public void BuildFindPredicateTemplate_ShouldBuildCorrectExpression_WhenEntityHasSimpleShadowKey()
  {
    // Arrange
    PredicateTemplateProvider sut = new();

    // Act
    LambdaExpression template = sut.GetFindPredicateTemplate<EntityWithSimpleShadowKey, int>(Models.SimpleShadowKeyModel);

    // Assert
    template.Should().BeEquivalentTo(FindPredicateTemplates.SimpleShadowKeyTemplate);
  }

  [Fact]
  public void BuildFindPredicateTemplate_ShouldBuildCorrectExpression_WhenEntityHasCompositeKey()
  {
    // Arrange
    PredicateTemplateProvider sut = new();

    // Act
    LambdaExpression template = sut.GetFindPredicateTemplate<EntityWithCompositeKey, (int, string)>(Models.CompositeKeyModel);

    // Assert
    template.Should().BeEquivalentTo(FindPredicateTemplates.CompositeKeyTemplate);
  }
}
