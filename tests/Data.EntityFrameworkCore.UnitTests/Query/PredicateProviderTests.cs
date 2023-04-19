using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

public partial class PredicateProviderTests
{
  [Theory]
  [EntityWithSimplePropertyKeyData]
  internal void GetFindPredicate_ShouldReturnCorrectPredicate_ForEntityWithSimplePropertyKey(IPredicateTemplateFactory templateFactory, Mock<IPredicateTemplateCache> cacheMock)
  {
    // Arrange
    const int id = 12;
    Expression<Func<EntityWithSimplePropertyKey, bool>> expected = entity => entity.Id == id;

    PredicateProvider sut = new(templateFactory, cacheMock.Object, Mock.Of<IModel>(MockBehavior.Strict));

    // Act
    var predicate = sut.GetFindPredicate<EntityWithSimplePropertyKey, int>(id);

    // Assert
    predicate.Should().BeEquivalentTo(expected);
    cacheMock.VerifyAll();
    cacheMock.VerifyNoOtherCalls();
  }

  [Theory]
  [EntityWithSimpleFieldKeyData]
  internal void GetFindPredicate_ShouldReturnCorrectPredicate_ForEntityWithSimpleFieldKey(IPredicateTemplateFactory templateFactory, Mock<IPredicateTemplateCache> cacheMock)
  {
    // Arrange
    const int id = 12;
    Expression<Func<EntityWithSimpleFieldKey, bool>> expected = entity => entity.Id == id;

    PredicateProvider sut = new(templateFactory, cacheMock.Object, Mock.Of<IModel>(MockBehavior.Strict));

    // Act
    var predicate = sut.GetFindPredicate<EntityWithSimpleFieldKey, int>(id);

    // Assert
    predicate.Should().BeEquivalentTo(expected);
    cacheMock.VerifyAll();
    cacheMock.VerifyNoOtherCalls();
  }

  [Theory]
  [EntityWithSimpleShadowKeyData]
  internal void GetFindPredicate_ShouldReturnCorrectPredicate_ForEntityWithSimpleShadowKey(IPredicateTemplateFactory templateFactory, Mock<IPredicateTemplateCache> cacheMock)
  {
    // Arrange
    const int id = 12;
    Expression<Func<EntityWithSimpleShadowKey, bool>> expected = entity => EF.Property<int>(entity, "Id") == id;

    PredicateProvider sut = new(templateFactory, cacheMock.Object, Mock.Of<IModel>(MockBehavior.Strict));

    // Act
    var predicate = sut.GetFindPredicate<EntityWithSimpleShadowKey, int>(id);

    // Assert
    predicate.Should().BeEquivalentTo(expected);
    cacheMock.VerifyAll();
    cacheMock.VerifyNoOtherCalls();
  }

  [Theory]
  [EntityWithCompositeKeyData]
  internal void GetFindPredicate_ShouldReturnCorrectPredicate_ForEntityWithCompositeKey(IPredicateTemplateFactory templateFactory, Mock<IPredicateTemplateCache> cacheMock)
  {
    // Arrange
    const int id1 = 12;
    const string id2 = "id2";
    Expression<Func<EntityWithCompositeKey, bool>> expected = entity => entity.Id1 == id1 && entity.Id2 == id2;

    PredicateProvider sut = new(templateFactory, cacheMock.Object, Mock.Of<IModel>(MockBehavior.Strict));

    // Act
    var predicate = sut.GetFindPredicate<EntityWithCompositeKey, (int, string)>((id1, id2));

    // Assert
    predicate.Should().BeEquivalentTo(expected);
    cacheMock.VerifyAll();
    cacheMock.VerifyNoOtherCalls();
  }
}
