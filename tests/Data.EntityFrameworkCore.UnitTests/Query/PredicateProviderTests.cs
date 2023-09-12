using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

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
  internal void GetFindPredicate_ShouldReturnCorrectPredicate_ForEntityWithSimplePropertyKey()
  {
    // Arrange
    const int id = 12;
    Expression<Func<EntityWithSimplePropertyKey, bool>> expected = entity => entity.Id == id;

    _templateProviderMock
      .Setup(x => x.GetFindPredicateTemplate<EntityWithSimplePropertyKey, int>(It.IsAny<IModel>()))
      .Returns(FindPredicateTemplates.SimplePropertyKeyTemplate);

    // Act
    var predicate = _sut.GetFindPredicate<EntityWithSimplePropertyKey, int>(Mock.Of<IModel>(), id);

    // Assert
    predicate.Should().BeEquivalentTo(expected);
  }

  [Fact]
  internal void GetFindPredicate_ShouldReturnCorrectPredicate_ForEntityWithSimpleFieldKey()
  {
    // Arrange
    const int id = 12;
    Expression<Func<EntityWithSimpleFieldKey, bool>> expected = entity => entity.Id == id;

    _templateProviderMock
      .Setup(x => x.GetFindPredicateTemplate<EntityWithSimpleFieldKey, int>(It.IsAny<IModel>()))
      .Returns(FindPredicateTemplates.SimpleFieldKeyTemplate);

    // Act
    var predicate = _sut.GetFindPredicate<EntityWithSimpleFieldKey, int>(Mock.Of<IModel>(), id);

    // Assert
    predicate.Should().BeEquivalentTo(expected);
  }

  [Fact]
  internal void GetFindPredicate_ShouldReturnCorrectPredicate_ForEntityWithSimpleShadowKey()
  {
    // Arrange
    const int id = 12;
    Expression<Func<EntityWithSimpleShadowKey, bool>> expected = entity => EF.Property<int>(entity, "Id") == id;

    _templateProviderMock
      .Setup(x => x.GetFindPredicateTemplate<EntityWithSimpleShadowKey, int>(It.IsAny<IModel>()))
      .Returns(FindPredicateTemplates.SimpleShadowKeyTemplate);

    // Act
    var predicate = _sut.GetFindPredicate<EntityWithSimpleShadowKey, int>(Mock.Of<IModel>(), id);

    // Assert
    predicate.Should().BeEquivalentTo(expected);
  }

  [Fact]
  internal void GetFindPredicate_ShouldReturnCorrectPredicate_ForEntityWithCompositeKey()
  {
    // Arrange
    const int id1 = 12;
    const string id2 = "id2";
    Expression<Func<EntityWithCompositeKey, bool>> expected = entity => entity.Id1 == id1 && entity.Id2 == id2;

    _templateProviderMock
      .Setup(x => x.GetFindPredicateTemplate<EntityWithCompositeKey, (int, string)>(It.IsAny<IModel>()))
      .Returns(FindPredicateTemplates.CompositeKeyTemplate);

    // Act
    var predicate = _sut.GetFindPredicate<EntityWithCompositeKey, (int, string)>(Mock.Of<IModel>(), (id1, id2));

    // Assert
    predicate.Should().BeEquivalentTo(expected);
  }
}
