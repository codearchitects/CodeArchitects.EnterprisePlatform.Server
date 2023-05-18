using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

public partial class PredicateProviderTests
{
  [Theory]
  [KeyPredicate]
  internal void GetPredicate_ShouldReturnCorrectPredicate_ForKey(IPredicateTemplateFactory templateFactory, Mock<IPredicateTemplateCache> cacheMock)
  {
    // Arrange
    Guid id = Guid.Empty;
    Expression<Func<EntityWithIdProperty, bool>> expected = entity => entity.Id == id;
    PredicateProvider sut = new(templateFactory, cacheMock.Object);
    EntityModel entityModel = EntityModel.Create(typeof(EntityWithIdProperty));

    // Act
    Expression<Func<EntityWithIdProperty, bool>> predicate = sut.GetPredicate<EntityWithIdProperty, Guid>(id, entityModel);

    // Assert
    predicate.Should().BeEquivalentTo(expected);
    cacheMock.VerifyAll();
    cacheMock.VerifyNoOtherCalls();
  }

  [Theory]
  [EntityPredicate]
  internal void GetPredicate_ShouldReturnCorrectPredicate_ForEntity(IPredicateTemplateFactory templateFactory, Mock<IPredicateTemplateCache> cacheMock)
  {
    // Arrange
    EntityWithIdProperty entity = new() { Id = Guid.Empty };
    Expression<Func<EntityWithIdProperty, bool>> expected = entity => entity.Id == entity.Id;
    PredicateProvider sut = new(templateFactory, cacheMock.Object);
    EntityModel entityModel = EntityModel.Create(typeof(EntityWithIdProperty));

    // Act
    Expression<Func<EntityWithIdProperty, bool>> predicate = sut.GetPredicate(entity, entityModel);

    // Assert
    predicate.Should().BeEquivalentTo(expected);
    cacheMock.VerifyAll();
    cacheMock.VerifyNoOtherCalls();
  }
}
