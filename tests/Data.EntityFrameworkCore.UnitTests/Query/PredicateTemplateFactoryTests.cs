using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

public class PredicateTemplateFactoryTests
{
  [Fact]
  public void BuildFindPredicateTemplate_ShouldBuildCorrectExpression_WhenEntityHasSimplePropertyKey()
  {
    // Arrange
    PredicateTemplateFactory sut = new(Models.SimplePropertyKeyModel);

    // Act
    Expression<Func<EntityWithSimplePropertyKey, bool>> template = sut.CreateFindPredicateTemplate<EntityWithSimplePropertyKey, int>();

    // Assert
    template.Should().BeEquivalentTo(FindPredicateTemplates.SimplePropertyKeyTemplate, options => options.Using(ExpressionEqualityComparer.Instance));
  }

  [Fact]
  public void BuildFindPredicateTemplate_ShouldBuildCorrectExpression_WhenEntityHasSimpleFieldKey()
  {
    // Arrange
    PredicateTemplateFactory sut = new(Models.SimpleFieldKeyModel);

    // Act
    Expression<Func<EntityWithSimpleFieldKey, bool>> template = sut.CreateFindPredicateTemplate<EntityWithSimpleFieldKey, int>();

    // Assert
    template.Should().BeEquivalentTo(FindPredicateTemplates.SimpleFieldKeyTemplate, options => options.Using(ExpressionEqualityComparer.Instance));
  }

  [Fact]
  public void BuildFindPredicateTemplate_ShouldBuildCorrectExpression_WhenEntityHasSimpleShadowKey()
  {
    // Arrange
    PredicateTemplateFactory sut = new(Models.SimpleShadowKeyModel);

    // Act
    Expression<Func<EntityWithSimpleShadowKey, bool>> template = sut.CreateFindPredicateTemplate<EntityWithSimpleShadowKey, int>();

    // Assert
    template.Should().BeEquivalentTo(FindPredicateTemplates.SimpleShadowKeyTemplate, options => options.Using(ExpressionEqualityComparer.Instance));
  }

  [Fact]
  public void BuildFindPredicateTemplate_ShouldBuildCorrectExpression_WhenEntityHasCompositeKey()
  {
    // Arrange
    PredicateTemplateFactory sut = new(Models.CompositeKeyModel);

    // Act
    Expression<Func<EntityWithCompositeKey, bool>> template = sut.CreateFindPredicateTemplate<EntityWithCompositeKey, (int, string)>();

    // Assert
    template.Should().BeEquivalentTo(FindPredicateTemplates.CompositeKeyTemplate, options => options.Using(ExpressionEqualityComparer.Instance));
  }
}
