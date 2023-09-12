using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

public partial class PredicateProviderTests
{
  private readonly Mock<IPredicateTemplateProvider> _templateProviderMock;
  private readonly PredicateProvider _sut;

  public PredicateProviderTests()
  {
    _templateProviderMock = new(MockBehavior.Strict);
    _sut = new(_templateProviderMock.Object);
  }

  [Fact]
  internal void GetPredicate_ShouldReturnCorrectPredicate_ForKey()
  {
    // Arrange
    Guid id = Guid.Empty;
    Expression<Func<EntityWithIdProperty, bool>> expected = entity => entity.Id == id;
    EntityModel entityModel = EntityModel.Create(typeof(EntityWithIdProperty));

    _templateProviderMock
      .Setup(x => x.GetFindPredicateTemplate<EntityWithIdProperty, Guid>(entityModel))
      .Returns(PredicateTemplates.EntityWithIdPropertyKeyTemplate);

    // Act
    Expression<Func<EntityWithIdProperty, bool>> predicate = _sut.GetFindPredicate<EntityWithIdProperty, Guid>(entityModel, id);

    // Assert
    predicate.Should().BeEquivalentTo(expected);
  }

  [Fact]
  internal void GetPredicate_ShouldReturnCorrectPredicate_ForEntity()
  {
    // Arrange
    EntityWithIdProperty entity = new() { Id = Guid.Empty };
    Expression<Func<EntityWithIdProperty, bool>> expected = entity => entity.Id == entity.Id;
    EntityModel entityModel = EntityModel.Create(typeof(EntityWithIdProperty));

    _templateProviderMock
      .Setup(x => x.GetFindPredicateTemplate<EntityWithIdProperty, Guid>(entityModel))
      .Returns(PredicateTemplates.EntityWithIdPropertyEntityTemplate);

    // Act
    Expression<Func<EntityWithIdProperty, bool>> predicate = _sut.GetFindPredicate(entityModel, entity);

    // Assert
    predicate.Should().BeEquivalentTo(expected);
  }
}
